namespace HogwartApi.Contracts;

public class FoodDiaryModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public IEnumerable<int> FoodIds { get; set; }
}