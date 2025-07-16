using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace ECommerce_App.Model
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonElement("Comments")]
        public string comments { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Productid")]
        public string productid { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Userid")]
        public string userid { get; set; }
    }

    public class CreateReviewDTO
    {
        public string comments { get; set; }
        public string productid{ get; set; }
        public string userid { get; set; }
    }
    public class ResponseReviewDTO
    {
        public string _id { get; set; }
        public string comments { get; set; }
        public string productid { get; set; }
        public string userid { get; set; }
    }
}
