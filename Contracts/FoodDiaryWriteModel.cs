using System.Text.Json.Serialization;

namespace HogwartApi.Contracts;

public sealed class FoodDiaryWriteModel
{
    [JsonPropertyName("foodIds")]
    public List<int> FoodIds { get; set; } = [];
    
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}