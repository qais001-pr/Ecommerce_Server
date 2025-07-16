namespace ECommerce_App.Model
{

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("price")]
        public float Price { get; set; }

        [BsonElement("rating")]
        public float Rating { get; set; } = 0;

        [BsonElement("imageBytes")]
        public byte[] ImageBytes { get; set; }

        [BsonElement("imageContentType")]
        public string ImageContentType { get; set; }

        [BsonElement("imageExtension")]
        public string ImageExtension { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("categoryid")]
        public string CategoryId { get; set; }

    }
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string CategoryId { get; set; }
        public string ImageBase64 { get; set; }
    }
    public class ProductResponseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string CategoryId { get; set; }
        public string ImageBase64 { get; set; }

    }
}
