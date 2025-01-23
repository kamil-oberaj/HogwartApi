using JetBrains.Annotations;

namespace HogwartApi.Core.Models;

public sealed class FoodDiary
{
    public int Id { get; }
    public DateTime Date { get; private init; }

    public ICollection<int> Foods { get; private set; } = [];

    [UsedImplicitly]
    private FoodDiary()
    {
    }
    
    public FoodDiary(DateTime date, params HashSet<int> foods)
    {
        Date = date;
        Foods = foods.ToList();
    }

    public static FoodDiary Create(DateTime date, params HashSet<int> foods)
    {
        return new FoodDiary(date, foods);
    }
}