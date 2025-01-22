using JetBrains.Annotations;

namespace HogwartApi.Core.Models;

public class Food
{
    public int Id { get; }
    public string Name { get; private set; }
    public int KCal { get; private set; }

    [UsedImplicitly]
    private Food()
    {
    }
    
    public Food(string name, int kCal)
    {
        Name = name;
        KCal = kCal;
    }
    
    public static Food Create(string name, int kCal)
    {
        return new Food(name, kCal);
    }
    
    public void Update(string name, int kCal)
    {
        Name = name;
        KCal = kCal;
    }
}