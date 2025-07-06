using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ECommerce_App.Model
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? id { get; set; }
        [BsonRequired]
        [BsonElement("name")]
        [MaxLength(20)]
        public string name { get; set; }

        [BsonElement("description")]
        [MaxLength(300)]
        public string description { get; set; }
        [BsonElement("TotalProduct")]
        public int products { get; set; }
    }

    public class CategoryDto
    {
        public string? id { get; set; }
        [Required]
        [MaxLength(20)]
        public string name { get; set; }

        [MaxLength(300)]
        public string description { get; set; }

        public int products { get; set; } = 0;
    }
}
