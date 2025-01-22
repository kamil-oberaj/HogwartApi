using HogwartApi.Contracts;
using HogwartApi.Core.Exceptions;
using HogwartApi.Core.Models;
using HogwartApi.Persistance;
using Microsoft.EntityFrameworkCore;

namespace HogwartApi.Services;

public sealed class EmployeesService(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<HashSet<EmployeeModel>> GetEmployeesAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var employees = await dbContext.Employees.Select(x => new EmployeeModel
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName
        }).ToListAsync(cancellationToken);

        return employees.ToHashSet();
    }
    
    public async Task AddEmployeeAsync(EmployeeWriteModel model, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var otherEmployeeExists = await dbContext.Employees.Where(x =>
                x.FirstName.ToLower() == model.FirstName.ToLower() &&
                x.LastName.ToLower() == model.LastName.ToLower())
            .AnyAsync(cancellationToken);
        
        if (otherEmployeeExists)
        {
            throw new ConflictException($"Employee with name {model.FirstName} {model.LastName} already exists.");
        }

        var employee = Employee.Create(model.FirstName, model.LastName);

        await dbContext.Employees.AddAsync(employee, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateEmployeeAsync(int id, EmployeeWriteModel model, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var otherEmployeeExists = await dbContext.Employees.Where(x =>
                x.Id != id &&
                x.FirstName.ToLower() == model.FirstName.ToLower() &&
                x.LastName.ToLower() == model.LastName.ToLower())
            .AnyAsync(cancellationToken);
        
        if (otherEmployeeExists)
        {
            throw new ConflictException($"Employee with name {model.FirstName} {model.LastName} already exists.");
        }

        var employee = await dbContext.Employees.FindAsync([id], cancellationToken);
        
        if (employee is null)
        {
            throw new NotFoundException($"Employee with id {id} not found.");
        }
        
        employee.Update(model.FirstName, model.LastName);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RemoveEmployeeAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var employee = await dbContext.Employees.FindAsync([id], cancellationToken);
        
        if (employee is null)
        {
            throw new NotFoundException($"Employee with id {id} not found.");
        }
        
        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}