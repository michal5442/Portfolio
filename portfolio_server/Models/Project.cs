using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using portfolio_server.Enums;


namespace portfolio_server.Models
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public int IdntAgaff { get; set; }

        public string Agaff { get; set; }

        public int IdntYechidaMevatzat { get; set; }

        public string YechidaMevatzat { get; set; }

        public string ProjectName { get; set; }

        public string Teur { get; set; }

        public Maslol Maslol { get; set; }

        public int IdntMaslol { get; set; }

        public bool LogHemsheci { get; set; }

        public int TotalTakzivCoachAdam { get; set; }

        public int TotalTakzivRechesh { get; set; }

        public int CoachAdam { get; set; }

        public string Hearot { get; set; }
        
        public bool Active { get; set; }
         
        public int Year { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}