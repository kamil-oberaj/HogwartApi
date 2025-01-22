namespace HogwartApi.Core.Configuration;

public sealed class DbContextConfiguration(string connectionString)
{
    private string _connectionString { get; } = connectionString;

    public string ConnectionString => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("HpgwartsDatabase"))
        ? Environment.GetEnvironmentVariable("HpgwartsDatabase")!
        : !string.IsNullOrWhiteSpace(_connectionString)
            ? _connectionString
            : throw new ArgumentNullException(nameof(_connectionString));
    
    public const string SectionName = "Database";
}