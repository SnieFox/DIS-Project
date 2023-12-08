using System.Text.Json;
using DISProject.Database.DatabaseContext;
using DISProject.Database.DTO;
using DISProject.Database.Entities;
using DISProject.Database.Services.KafkaServices;
using Microsoft.EntityFrameworkCore;

namespace DISProject.Database.Services.PurchaseServices;

public class PurchaseService : IPurchaseService
{
    private readonly DISProjectContext _context;
    private readonly IProducerService _kafkaProducer;

    public PurchaseService(DISProjectContext context, IProducerService kafkaProducer)
    {
        _context = context;
        _kafkaProducer = kafkaProducer;
    }
    public async Task<(bool IsSuccess, string Message, int OrderId)> ExecutePurchaseAsync(int id, int quantity)
    {
        var purchase = new Purchase
        {
            ProductId = id,
            Quantity = quantity,
            Status = "Pending"
        };
        
        await _context.Purchases.AddAsync(purchase);
        var added = await _context.SaveChangesAsync();
        if (added == 0)
            return (false, "Something went wrong when adding Purchase to queue", 0);

        int orderId = await _context.Purchases
            .OrderByDescending(p => p.Id)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();
        return (true, string.Empty, orderId);
    }
    
    // Processing of the purchase queue
    public async Task ProcessQueueAsync()
    {
        var pendingPurchase = await _context.Purchases
            .Where(p => p.Status == "Pending")
            .OrderBy(p => p.Id)
            .FirstOrDefaultAsync();

        if (pendingPurchase != null)
        {
            var availability = await CheckProductAvailabilityAsync(pendingPurchase.ProductId, pendingPurchase.Quantity);
            if (availability.IsAvailable)
            {
                await ProcessPurchase(pendingPurchase.ProductId, pendingPurchase.Quantity);
                await SetPurchaseStatus(pendingPurchase, "Completed");

                string kafkaMessage = JsonSerializer.Serialize(new KafkaObjectResponse
                {
                    Id = pendingPurchase.Id,
                    Message = "The order is accepted."
                });
                await _kafkaProducer.ProduceAsync("demo", kafkaMessage);
            }
            else
            {
                await SetPurchaseStatus(pendingPurchase, "Failed");
                
                string kafkaMessage = JsonSerializer.Serialize(new KafkaObjectResponse
                {
                    Id = pendingPurchase.Id,
                    Message = $"{availability.Message}."
                });
                await _kafkaProducer.ProduceAsync("demo", kafkaMessage);
            }
        }
    }
    
    // Check product availability before buying
    private async Task<(bool IsAvailable, string Message)> CheckProductAvailabilityAsync(int productId, int quantity)
    {
        var product = await _context.People
            .Where(p => p.Id == productId)
            .FirstOrDefaultAsync();
        
        if (product.Quantity <= 0)
            return (false, "The product is out of stock");

        if (product.Quantity - quantity < 0)
            return (false, $"The required quantity of the item is not available, the available quantity is {product.Quantity}");

        return (true, string.Empty);
    }

    // The logic of processing the purchase itself
    private async Task ProcessPurchase(int productId, int quantity)
    {
        var product = await _context.People
            .Where(p => p.Id == productId)
            .FirstOrDefaultAsync();
        if (product != null)
        {
            var newQuantity = product.Quantity - quantity;
            product.Quantity = newQuantity;
            await _context.SaveChangesAsync();
        }
    }

    // Update the status of your purchase
    private async Task SetPurchaseStatus(Purchase purchase, string status)
    {
        purchase.Status = status;
        await _context.SaveChangesAsync();
    }
}