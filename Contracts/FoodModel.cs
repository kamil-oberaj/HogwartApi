using System.Text.Json.Serialization;

namespace HogwartApi.Contracts;

public sealed class FoodModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("kcal")]
    public int KCal { get; set; }
}