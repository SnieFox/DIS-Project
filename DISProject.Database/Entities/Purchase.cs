namespace DISProject.Database.Entities;

public class Purchase
{
    public string Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; }
}