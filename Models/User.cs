using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ushopDN.Models

{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("email")]
        public required string Email { get; set; }

        [BsonElement("password")]
        public required string Password { get; set; }

        [BsonElement("cartData")]
        public Dictionary<string, int>? CartData { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }
    }
}