using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace ECommerce_App.Model
{
    public class whishlists
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string? _id { get; set; }
        [BsonElement("isFavourite")]
        public bool isFavourite { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userid")]
        public string userid { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("productid")]
        public string productid { get; set; }
    }

    public class whishlistsCreateDTO
    {
        public bool isFavourite { get; set; } = true;
        public string productid { get; set; }
        public string userid { get; set; }
    }

    public class whishlistsResponseDTO
    {
        public string? _id { get; set; }
        public bool isFavourite { get; set; }
        public string productid { get; set; }
        public string userid { get; set; }
    }
}
