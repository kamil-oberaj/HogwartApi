using System.Text.Json.Serialization;

namespace HogwartApi.Contracts;

public sealed class ParkingLotWriteModel
{
    [JsonPropertyName("employeeId")]
    public int EmployeeId { get; set; }
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }
    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }
}