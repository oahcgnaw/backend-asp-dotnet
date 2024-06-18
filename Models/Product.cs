using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ushopDN.Models
{
    [BsonIgnoreExtraElements]
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("image")]
        public required string Image { get; set; }

        [BsonElement("category")]
        public required string Category { get; set; }

        [BsonElement("new_price")]
        public double New_price { get; set; }

        [BsonElement("old_price")]
        public double Old_price { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [BsonElement("available")]
        public bool Available { get; set; } = true;

    }
}