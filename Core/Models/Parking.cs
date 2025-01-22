using JetBrains.Annotations;

namespace HogwartApi.Core.Models;

public sealed class Parking
{
    public int Id { get; }
    public int EmployeeId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    [UsedImplicitly]
    private Parking()
    {
        
    }
    
    public Parking(int employeeId, DateTime startTime, DateTime endTime)
    {
        EmployeeId = employeeId;
        StartTime = startTime;
        EndTime = endTime;
    }
    
    public void Update(int employeeId, DateTime startTime, DateTime endTime)
    {
        EmployeeId = employeeId;
        StartTime = startTime;
        EndTime = endTime;
    }
    
    public static Parking Create(int employeeId, DateTime startTime, DateTime endTime)
    {
        return new Parking(employeeId, startTime, endTime);
    }
}