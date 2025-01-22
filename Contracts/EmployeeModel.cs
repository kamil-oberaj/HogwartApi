using System.Text.Json.Serialization;

namespace HogwartApi.Contracts;

public sealed class EmployeeModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("firstName")] public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")] public string LastName { get; set; } = string.Empty;
}