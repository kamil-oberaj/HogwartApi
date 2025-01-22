using System.Text.Json.Serialization;

namespace HogwartApi.Contracts;

public sealed class EmployeeWriteModel
{
    [JsonPropertyName("firstName")] public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")] public string LastName { get; set; } = string.Empty;
}