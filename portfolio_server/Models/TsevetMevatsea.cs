using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using portfolio_server.Enums;
using System.Text.Json.Serialization;


namespace portfolio_server.Models
{
    public class TsevetMevatsea
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid IdntTsevetMevatsea { get; set; }

        public string TsevetMevatseaName { get; set; }

        public bool Active { get; set; } = true;
    }
}