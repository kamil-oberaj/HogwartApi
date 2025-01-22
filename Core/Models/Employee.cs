using JetBrains.Annotations;

namespace HogwartApi.Core.Models;

public sealed class Employee
{
    public int Id { get; set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    [UsedImplicitly]
    private Employee()
    {
    }
    
    public Employee(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
    
    public static Employee Create(string firstName, string lastName)
    {
        return new Employee(firstName, lastName);
    }
    
    public void Update(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}