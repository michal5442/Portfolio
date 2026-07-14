using System;
using System.Text.Json.Serialization;

namespace portfolio_server.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Maslol
    {
        KIYUM,
        HITAZMUT
    }
}
