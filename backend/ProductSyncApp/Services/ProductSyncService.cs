using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProductSyncService
{
    private readonly HttpClient _httpClient;

    public ProductSyncService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ดึงข้อมูลสินค้า
    public async Task<List<Product>> SyncProductsAsync(int page, int pageSize)
    {
        var body = new { Page = page, PageSize = pageSize };
        var response = await _httpClient.PostAsJsonAsync("https://tabledusud.nl/_product/simpleFilters", body);

        // ตรวจสอบสถานะของการตอบกลับ
        response.EnsureSuccessStatusCode();

        // อ่านข้อมูล JSON ที่ส่งกลับมาเป็น ProductResponse
        var productResponse = await response.Content.ReadFromJsonAsync<ProductResponse>();

        // ตรวจสอบว่ามีข้อมูลสินค้า
        if (productResponse == null || productResponse.Products == null || productResponse.Products.Count == 0)
        {
            throw new InvalidOperationException("Failed to parse products from API or no products found.");
        }

        // แปลง ProductData เป็น Product
        var products = new List<Product>();
        foreach (var productData in productResponse.Products)
        {
            // แสดง ThumbnailImage ใน Console
            Console.WriteLine($"Product Name: {productData.Name}, ThumbnailImage: {productData.ThumbnailImage}");
            products.Add(productData.ToProduct());
        }

        return products;
    }
}

// คลาสสำหรับการตอบกลับ JSON จาก API
public class ProductResponse
{
    public List<ProductData> Products { get; set; } = new List<ProductData>();
}
