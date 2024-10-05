using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Product
{
    // กำหนดให้ Id เป็น ObjectId เพื่อให้ MongoDB จัดการได้ง่ายขึ้น
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Name { get; set; }
    public Price? Price { get; set; }
    
    // ปรับ ImageUrl ให้ดึงจาก ThumbnailImage
    public string? ImageUrl { get; set; } 
    
    // ปรับ CreatedDate ให้ดึงจาก Created
    public DateTime CreatedDate { get; set; } 
}

public class Price
{
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
}
