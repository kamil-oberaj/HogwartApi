using HogwartApi.Contracts;
using HogwartApi.Core.Exceptions;
using HogwartApi.Core.Models;
using HogwartApi.Persistance;
using Microsoft.EntityFrameworkCore;

namespace HogwartApi.Services;

public class FoodsService(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<HashSet<FoodModel>> GetFoodsAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var foods = await dbContext.Foods.Select(x => new FoodModel
        {
            Id = x.Id,
            Name = x.Name,
            KCal = x.KCal
        }).ToListAsync(cancellationToken);

        return foods.ToHashSet();
    }
    
    public async Task AddFoodAsync(FoodWriteModel model, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var food = Food.Create(model.Name, model.KCal);

        await dbContext.Foods.AddAsync(food, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateFoodAsync(int id, FoodWriteModel model, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var otherFoodExists = await dbContext.Foods.Where(x => 
                x.Id != id && x.Name.ToLower() == model.Name.ToLower())
                    .AnyAsync(cancellationToken);
        
        if (otherFoodExists)
        {
            throw new ConflictException($"Food with name {model.Name} already exists.");
        }

        var food = await dbContext.Foods.FindAsync([id], cancellationToken);
        
        if (food is null)
        {
            throw new NotFoundException($"Food with id {id} not found.");
        }
        
        food.Update(model.Name, model.KCal);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RemoveFoodAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var food = await dbContext.Foods.FindAsync([id], cancellationToken);
        
        if (food is null)
        {
            throw new NotFoundException($"Food with id {id} not found.");
        }
        
        dbContext.Foods.Remove(food);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}