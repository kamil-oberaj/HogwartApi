using System.Text.Json.Serialization;

namespace HogwartApi.Contracts;

public sealed class FoodWriteModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("kcal")]
    public int KCal { get; set; }
}