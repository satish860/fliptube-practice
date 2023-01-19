using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FilpTube.History
{
    public class VideoViewed
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? Id { get; set; }

        public string VideoPath { get; set; }
    }
}