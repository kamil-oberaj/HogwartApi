using HogwartApi.Contracts;
using HogwartApi.Core.Configuration;
using HogwartApi.Core.Exceptions;
using HogwartApi.Persistance;
using HogwartApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var dbConfig = builder.Configuration.GetSection(DbContextConfiguration.SectionName).Get<DbContextConfiguration>();

ArgumentNullException.ThrowIfNull(dbConfig, nameof(dbConfig));


builder.Services.AddDbContextFactory<AppDbContext>(opt =>
{
    opt.UseSqlServer(dbConfig.ConnectionString,
        cfg => { cfg.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery); });
});

builder.Services.AddSingleton<EmployeesService>();
builder.Services.AddSingleton<FoodDiariesService>();
builder.Services.AddSingleton<FoodsService>();
builder.Services.AddSingleton<ParkingService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapOpenApi();
app.MapScalarApiReference(opt =>
{
    opt.WithTitle("Hogwarts API")
        .WithTheme(ScalarTheme.Moon)
        .WithDefaultHttpClient(ScalarTarget.Http, ScalarClient.Curl);
});

app.MapGet("api/food-diary", async (int? foodId, FoodDiariesService service, CancellationToken cancellationToken) =>
    {
        var data = await service.GetFoodDiaries(foodId, cancellationToken);

        return Results.Ok(data);
    })
    .Produces<Ok<HashSet<FoodDiaryModel>>>();

app.MapPost("api/diet-diary-entries",
    async (FoodDiaryWriteModel model, FoodDiariesService service, CancellationToken cancellationToken) =>
    {
        await service.AddFoodDiaryAsync(model, cancellationToken);

        return Results.Ok();
    })
    .Produces<Ok>();

app.MapDelete("api/diet-diary-entries",
    async (int id, FoodDiariesService service, CancellationToken cancellationToken) =>
    {
        try
        {
            await service.RemoveFoodDiaryAsync(id, cancellationToken);

            return Results.Ok();
        }
        catch (NotFoundException)
        {
            return Results.NotFound(id);
        }
    })
    .Produces<Ok>()
    .Produces<NotFound>();


app.MapGet("api/foods", async (FoodsService service, CancellationToken cancellationToken) =>
    {
        var data = await service.GetFoodsAsync(cancellationToken);

        return Results.Ok(data);
    })
    .Produces<Ok<HashSet<FoodModel>>>();

app.MapPost("api/foods", async (FoodWriteModel model, FoodsService service, CancellationToken cancellationToken) =>
    {
        await service.AddFoodAsync(model, cancellationToken);

        return Results.Ok();
    }).Produces<Ok>();

app.MapPut("api/foods",
        async (int id, FoodWriteModel model, FoodsService service, CancellationToken cancellationToken) =>
        {
            try
            {
                await service.UpdateFoodAsync(id, model, cancellationToken);

                return Results.Ok();
            }
            catch (NotFoundException)
            {
                return Results.NotFound(id);
            }
            catch (ConflictException ex)
            {
                return Results.Conflict(ex.Message);
            }
        })
    .Produces<Ok>()
    .Produces<NotFound>()
    .Produces<Conflict>();


app.MapDelete("api/foods", async (int id, FoodsService service, CancellationToken cancellationToken) =>
    {
        try
        {
            await service.RemoveFoodAsync(id, cancellationToken);

            return Results.Ok();
        }
        catch (NotFoundException)
        {
            return Results.NotFound(id);
        }
    })
    .Produces<Ok>()
    .Produces<NotFound>();

app.MapGet("api/employees", async (EmployeesService service, CancellationToken cancellationToken) =>
{
    var data = await service.GetEmployeesAsync(cancellationToken);

    return Results.Ok(data);
}).Produces<Ok<HashSet<EmployeeModel>>>();

app.MapPost("api/employees",
        async (EmployeeWriteModel model, EmployeesService service, CancellationToken cancellationToken) =>
        {
            await service.AddEmployeeAsync(model, cancellationToken);

            return Results.Ok();
        })
    .Produces<Ok>()
    .Produces<Conflict>();

app.MapPut("api/employees", async (int id, EmployeeWriteModel model, EmployeesService service, CancellationToken cancellationToken) =>
    {
        try
        {
            await service.UpdateEmployeeAsync(id, model, cancellationToken);

            return Results.Ok();
        }
        catch (NotFoundException)
        {
            return Results.NotFound(id);
        }
        catch (ConflictException ex)
        {
            return Results.Conflict(ex.Message);
        }
    })
    .Produces<Ok>()
    .Produces<NotFound>()
    .Produces<Conflict>();


app.MapDelete("api/employees", async (int id, EmployeesService service, CancellationToken cancellationToken) =>
    {
        try
        {
            await service.RemoveEmployeeAsync(id, cancellationToken);

            return Results.Ok();
        }
        catch (NotFoundException)
        {
            return Results.NotFound(id);
        }
    })
    .Produces<Ok>()
    .Produces<NotFound>();


app.MapGet("api/parking-lots", async (DateOnly? startTimeFrom, ParkingService service, CancellationToken cancellationToken) =>
{
    var data = await service.GetParkingLotsAsync(startTimeFrom, cancellationToken);

    return Results.Ok(data);
}).Produces<Ok<HashSet<ParkingLotModel>>>();

app.MapPost("api/parking-lots", async (ParkingLotWriteModel model, ParkingService service, CancellationToken cancellationToken) =>
    {
        await service.AddParkingLotAsync(model, cancellationToken);

        return Results.Ok();
    })
    .Produces<Ok>();


app.MapDelete("api/parking-lots", async (int id, ParkingService service, CancellationToken cancellationToken) =>
{
    try
    {
        await service.RemoveParkingLotAsync(id, cancellationToken);

        return Results.Ok();
    }
    catch (NotFoundException)
    {
        return Results.NotFound(id);
    }
})
.Produces<Ok>()
.Produces<NotFound>();


app.UseHttpsRedirection();

await using (var dbContext = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext())
{
    await dbContext.Database.MigrateAsync();
}

await app.RunAsync();