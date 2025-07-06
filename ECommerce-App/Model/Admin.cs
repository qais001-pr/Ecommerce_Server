using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

public class Admin
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [BsonElement("Username")]
    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [BsonElement("Email")]
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [BsonElement("ContactNo")]
    [Required]
    [Phone]
    public string ContactNo { get; set; }

    [BsonElement("Gender")]
    [Required]
    public string Gender { get; set; }

    [BsonElement("Password")]
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [BsonIgnore]
    public IFormFile? ImageFile { get; set; }
    
    [BsonElement("Content")]
    public string? Content { get; set; }

    [BsonIgnore]
    public string? ImageBase64 { get; set; }

    [BsonElement("Image")]
    public byte[]? ImageBytes { get; set; }
}

public class AdminFormRequest
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public string Username { get; set; }
    [Required][EmailAddress] public string Email { get; set; }
    [Required] public string ContactNo { get; set; }
    [Required] public string Gender { get; set; }
    [Required] public string Password { get; set; }
    public IFormFile? ImageFile { get; set; }
    public string? ImageBase64 { get; set; }
    public string? Content { get; set; } 

    public byte[]? ImageBytes { get; set; }
}
public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }


}