using System.Text.Json.Serialization;

namespace Data.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Practice
    {
        None,
        Smile,
        MolarOpposites,
        FlossBoss,
        BiteMe,
        ThrillOfTheDrill
    }
}
