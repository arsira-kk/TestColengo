using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MongoDbContext
{
    private readonly IMongoCollection<Product> _products;

    public MongoDbContext(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("MongoDb"));
        var database = client.GetDatabase("ProductDb");
        _products = database.GetCollection<Product>("Products");
    }

    public async Task SaveProductsAsync(List<Product> products)
    {
        try
        {
            await _products.InsertManyAsync(products);
        }
        catch (MongoBulkWriteException<Product> ex)
        {
            // จัดการข้อผิดพลาดที่เกิดจากการเขียนข้อมูล
            // เช่น Duplicate Key Error
            foreach (var error in ex.WriteErrors)
            {
                // แสดงข้อความหรือบันทึกข้อผิดพลาด
                Console.WriteLine($"Error: {error.Message}");
            }
        }
        catch (Exception ex)
        {
            // จัดการข้อผิดพลาดทั่วไป
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public async Task<List<Product>> GetProductsAsync(int page, int pageSize)
    {
        return await _products
            .Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(string id)
    {
        return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateProductAsync(string id, Product updatedProduct)
    {
        await _products.ReplaceOneAsync(p => p.Id == id, updatedProduct);
    }

    public async Task DeleteProductAsync(string id)
    {
        await _products.DeleteOneAsync(p => p.Id == id);
    }
}
