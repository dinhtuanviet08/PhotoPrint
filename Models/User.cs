using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PhotoPrintAPI.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } = "Customer"; // có thể là Admin
}

