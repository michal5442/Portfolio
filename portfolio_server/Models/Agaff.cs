using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using portfolio_server.Enums;
using System.Text.Json.Serialization;


namespace portfolio_server.Models
{
    public class Agaff
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid IdntAgaff { get; set; }

        public string AgaffName { get; set; }

        public bool Active { get; set; } = true;
    }
}