using HogwartApi.Contracts;
using HogwartApi.Core.Exceptions;
using HogwartApi.Core.Models;
using HogwartApi.Persistance;
using Microsoft.EntityFrameworkCore;

namespace HogwartApi.Services;

public sealed class ParkingService(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<HashSet<ParkingLotModel>> GetParkingLotsAsync(DateOnly? date, CancellationToken cancellationToken = default)
    {
        var dateTime = date?.ToDateTime(TimeOnly.Parse("00:00PM"), DateTimeKind.Utc);
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var parkingLots = await dbContext.Parkings
            .Where(x => !dateTime.HasValue || x.StartTime.Date >= dateTime.Value)
            .Select(x => new ParkingLotModel
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
            })
            .ToListAsync(cancellationToken);
        return parkingLots.ToHashSet();
    }
    
    public async Task AddParkingLotAsync(ParkingLotWriteModel model, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var parkingLot = Parking.Create(model.EmployeeId, model.StartTime, model.EndTime);
        dbContext.Parkings.Add(parkingLot);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RemoveParkingLotAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var parkingLot = await dbContext.Parkings.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        
        if (parkingLot is null)
        {
            throw new NotFoundException($"Parking lot with id {id} not found");
        }
        
        dbContext.Parkings.Remove(parkingLot);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}