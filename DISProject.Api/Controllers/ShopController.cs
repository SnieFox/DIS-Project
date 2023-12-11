using DISProject.Api.Models.DTO;
using DISProject.Database.Services;
using DISProject.Database.Services.PeopleServices;
using DISProject.Database.Services.PurchaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DISProject.Api.Controllers;

[ApiController]
[Route($"shop")]
public class ShopController : Controller
{
    private readonly IPeopleService _peopleService;
    private readonly IPurchaseService _purchaseService;
    public ShopController(IPeopleService peopleService, IPurchaseService purchaseService)
    {
        _peopleService = peopleService;
        _purchaseService = purchaseService;
    }

    [HttpGet]
    [Route("products")]
    //[Authorize]
    [ResponseCache(Duration = 5)]
    public async Task<IActionResult> GetAllProducts()
    {
        var allPeople = await _peopleService.GetAllPeopleAsync();
        if (!allPeople.IsSuccess)
            return BadRequest(allPeople.ErrorMessage);

        return Ok(allPeople.PeopleList);
    }
    
    [HttpPost]
    [Route("purchase")]
    //[Authorize]
    public async Task<IActionResult> PurchaseProduct(PurchaseDataRequest purchaseData)
    {
        var purchase = await _purchaseService.ExecutePurchaseAsync(purchaseData.Id, purchaseData.Quantity);
        if (!purchase.IsSuccess)
            return BadRequest(purchase.Message);

        return Ok(new PurchaseDataResponse {OrderId = purchase.OrderId});
    }
}