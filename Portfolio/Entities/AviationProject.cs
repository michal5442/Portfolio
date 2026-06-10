using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Portfolio.Entities
{
    public class AviationProject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public int IdntAgaff { get; set; }

        public string Agaff { get; set; }

        public int IdntYechidaMevatzat { get; set; }

        public string YechidaMevatzat { get; set; }

        public string Project { get; set; }

        public string Teur { get; set; }

        public string Maslol { get; set; }

        public int IdntMaslol { get; set; }

        public bool LogHemsheci { get; set; }

        public int TotalTakzuvCoachAdam { get; set; }

        public int TotalTakzivRechesh { get; set; }

        public int CoachAdam { get; set; }

        public string Hearot { get; set; }
    }
}