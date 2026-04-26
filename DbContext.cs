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

    public void GetAllProducts()
    {
        using var db = new SqliteConnection(_connectionString);
        db.Open();

        db.Query<Product>(
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

    public IEnumerable<Product> GetProductsInStock()
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
            WHERE quantity > 0 
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

        var product = db.QueryFirstOrDefault<Product>(
            """
            SELECT 
                id AS Id,
                name AS Name,
                unit_of_measure AS UnitOfMeasure,
                price AS Price,
                quantity AS Quantity,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM main.table_products
            WHERE id = @Id
            """,
            new { Id = productId });

        if (product == null) return false;
        if (quantityToSell > product.Quantity) return false;

        db.Execute(
            """
            UPDATE table_products 
            SET quantity = quantity - @Quantity 
            WHERE id = @Id
            """,
            new { Quantity = quantityToSell, Id = productId });

        var totalAmount = quantityToSell * product.Price;

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
        
        var remainingQuantity = product.Quantity - quantityToSell;
        
        return remainingQuantity != product.Quantity;
    }
}