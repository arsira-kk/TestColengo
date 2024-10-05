public class ProductData
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public Price? Price { get; set; }
    public string? ThumbnailImage { get; set; } // รับค่าจาก ThumbnailImage
    public DateTime Created { get; set; } // รับค่าจาก Created

    // แปลงเป็น Product
    public Product ToProduct()
    {
        return new Product
        {
            Id = this.Id,
            Name = this.Name,
            Price = this.Price,
            ImageUrl = this.ThumbnailImage, // ตั้งค่า ImageUrl จาก ThumbnailImage
            CreatedDate = this.Created // ตั้งค่า CreatedDate
        };
    }
}