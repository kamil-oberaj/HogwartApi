using HogwartApi.Contracts;
using HogwartApi.Core.Exceptions;
using HogwartApi.Core.Models;
using HogwartApi.Persistance;
using Microsoft.EntityFrameworkCore;

namespace HogwartApi.Services;

public sealed class FoodDiariesService(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task AddFoodDiaryAsync(FoodDiaryWriteModel model, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var foodDiary = FoodDiary.Create(model.Date, model.FoodIds.ToHashSet());

        await dbContext.FoodDiaries.AddAsync(foodDiary, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RemoveFoodDiaryAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var foodDiary = await dbContext.FoodDiaries.FindAsync([id], cancellationToken);
        
        if (foodDiary is null)
        {
            throw new NotFoundException($"Food diary with id {id} not found.");
        }
        
        dbContext.FoodDiaries.Remove(foodDiary);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<HashSet<FoodDiaryModel>> GetFoodDiaries(
        int? foodId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var foodDiaries = await dbContext.FoodDiaries
            .Where(x => !foodId.HasValue || x.Foods.Contains(foodId.Value))
            .Select(x => new FoodDiaryModel
            {
                Id = x.Id,
                Date = x.Date,
                FoodIds = x.Foods
            })
            .ToListAsync(cancellationToken);

        return foodDiaries.ToHashSet();
    }
}