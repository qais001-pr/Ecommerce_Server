using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
namespace ECommerce_App.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string? _id { get; set; }
        [Required]
        [BsonElement("Name")]
        public string name { get; set; }
        [Required]
        [BsonElement("ContactNo")]
        public string contactno { get; set; }
        [Required]
        [BsonElement("Gender")]
        public string gender { get; set; }
        [Required]
        [BsonElement("LocalAddress")]
        public string LocalAddress { get; set; }
        [Required]
        [BsonElement("Email")]
        public string email { get; set; }
        [Required]
        [BsonElement("Password")]
        public string password { get; set; }
    }
    public class CreateUserDTO
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string contactno { get; set; }
        [Required]
        public string gender { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string localaddress { get; set; }
    }
    public class ResponseUserDTO
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string contactno { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string localaddress { get; set; }
    }
}
