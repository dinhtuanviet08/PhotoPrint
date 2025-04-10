using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PhotoPrintAPI.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("ImageUrl")]
        public string ImageUrl { get; set; }

        [BsonElement("Quantity")]
        public int Quantity { get; set; }

        [BsonElement("Size")]
        public string Size { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; } = "Pending";

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
