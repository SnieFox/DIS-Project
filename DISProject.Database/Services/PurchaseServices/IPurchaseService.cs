namespace DISProject.Database.Services.PurchaseServices;

public interface IPurchaseService
{
    Task<(bool IsSuccess, string Message, int OrderId)> ExecutePurchaseAsync(int id, int quantity);
    Task ProcessQueueAsync();
}