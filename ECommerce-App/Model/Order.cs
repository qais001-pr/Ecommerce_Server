using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerce_App.Model
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }
        [BsonElement("Name")]
        public string name { get; set; }
        [BsonElement("TotalPrice")]
        public decimal TotalPrice { get; set; }
        [BsonElement("Quantity")]
        public int quantity { get; set; }
        [BsonElement("ShippingAddress")]
        public string shippingAddress { get; set; }


        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Productid")]
        public string productid { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Userid")]
        public string userid { get; set; }
    }
    public class CreateOrderDTO
    {
        public string name { get; set; }
        public decimal totalprice { get; set; }
        public string shippingAddress { get; set; }
        public int quantity { get; set; }
        public string productid { get; set; }
        public string userid { get; set; }
    }
}
