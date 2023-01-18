using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FilpTube.API
{
    public class Video
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? Id { get; set; }

        public string Path { get; set; }
    }
}
