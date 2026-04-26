namespace SubShop;

public record class Product
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string UnitOfMeasure { get; init; }
    public required decimal Price { get; init; }
    public int Quantity { get; init; }
    public required string CreatedAt { get; init; }
    public required string UpdatedAt { get; init; }
}