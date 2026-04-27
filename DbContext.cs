using Dapper;
using Microsoft.Data.Sqlite;

namespace SubShop;

public class DbContext
{
    private readonly string _connectionString;

    public DbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<Product> GetAllProducts()
    {
        using var db = new SqliteConnection(_connectionString);

        return db.Query<Product>(
            """
            SELECT 
            id AS Id,
            name AS Name,
            unit_of_measure AS UnitOfMeasure,
            price AS Price,
            quantity AS Quantity,
            created_at AS CreatedAt,
            updated_at AS UpdatedAt
            FROM table_products 
            ORDER BY id
            """
        );
    }

    public Product? GetProductById(int id)
    {
        using var db = new SqliteConnection(_connectionString);
        return db.QueryFirstOrDefault<Product>(
            """
            SELECT 
                id AS Id,
                name AS Name,
                unit_of_measure AS UnitOfMeasure,
                price AS Price,
                quantity AS Quantity,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM table_products 
            WHERE id = @Id
            """,
            new { Id = id });
    }

    public bool SellProduct(int productId, int quantityToSell)
    {
        using var db = new SqliteConnection(_connectionString);
        db.Open();

        var oldQuantity = db.ExecuteScalar<int>(
            """
            SELECT quantity 
            FROM table_products 
            WHERE id = @Id
            """,
            new { Id = productId });

        if (oldQuantity == 0)
            return false;

        if (quantityToSell > oldQuantity)
            return false;

        db.Execute(
            """
            UPDATE table_products 
            SET quantity = quantity - @Quantity 
            WHERE id = @Id
            """,
            new { Quantity = quantityToSell, Id = productId });

        var newQuantity = db.ExecuteScalar<int>(
            """
            SELECT quantity 
            FROM table_products 
            WHERE id = @Id
            """,
            new { Id = productId });

        var product = GetProductById(productId);
        var totalAmount = quantityToSell * product!.Price;

        db.Execute(
            """
            INSERT INTO table_sales (product_id, quantity, price, total_amount) 
            VALUES (@ProductId, @Quantity, @Price, @TotalAmount)
            """,
            new
            {
                ProductId = productId,
                Quantity = quantityToSell,
                Price = product.Price,
                TotalAmount = totalAmount
            });

        return newQuantity != oldQuantity;
    }
}