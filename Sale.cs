namespace SubShop;

public record class Sale
{
    public  int Id { get; init; }
    public required int ProductId { get; init; }
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal Price { get; init; }
    public required decimal TotalAmount { get; init; }
    public string? SaleDate { get; init; }
}