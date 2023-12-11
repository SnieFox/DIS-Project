namespace DISProject.Database.DTO;

public class KafkaObjectResponse
{
    public string orderId { get; set; }
    public int productId { get; set; }
    public string productName { get; set; }
    public int quantity { get; set; }
    public string message { get; set; }
    public string status { get; set; }
}