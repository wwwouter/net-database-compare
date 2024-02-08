using PetaPoco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmployeeProjectRepositoryPetaPoco : IEmployeeProjectRepository
{
    private readonly Database _database;

    public EmployeeProjectRepositoryPetaPoco(Database database)
    {
        _database = database;
    }

    public async Task AddEmployee(EmployeeAddDto employeeDto)
    {
        // Assuming EmployeeAddDto matches your Employee table structure and naming conventions
        var sql = @"
        INSERT INTO Employees (Id, Name, Age, Department, HireDate, Salary, AddressLine1, AddressLine2, City, CreatedOn, UpdatedOn)
        VALUES (@Id, @Name, @Age, @Department, @HireDate, @Salary, @AddressLine1, @AddressLine2, @City, @CreatedOn, @UpdatedOn)";

        await _database.ExecuteAsync(sql, new
        {
            Id = employeeDto.EmployeeID,
            Name = employeeDto.Name,
            Age = employeeDto.Age,
            Department = employeeDto.Department,
            HireDate = employeeDto.HireDate,
            Salary = employeeDto.Salary,
            AddressLine1 = employeeDto.AddressLine1,
            AddressLine2 = employeeDto.AddressLine2,
            City = employeeDto.City,
            CreatedOn = employeeDto.CreatedOn,
            UpdatedOn = employeeDto.UpdatedOn ?? DateTime.UtcNow, // Assuming UpdatedOn is nullable and defaults to UtcNow if not provided
        });
    }

    public async Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate)
    {
        // SQL statement to update the employee's name and UpdatedOn timestamp based on the ID
        var sql = @"
        UPDATE Employees
        SET Name = @Name,
            UpdatedOn = @UpdatedOn
        WHERE Id = @Id";

        // Execute the update operation
        await _database.ExecuteAsync(sql, new
        {
            Name = employeeUpdate.Name,
            UpdatedOn = DateTime.UtcNow, // Set UpdatedOn to the current UTC date and time
            Id = employeeUpdate.EmployeeID
        });
    }

    public async Task DeleteEmployeeById(EmployeeDeleteDto employeeDelete)
    {
        // SQL statement to delete an employee based on the ID
        var sql = @"
        DELETE FROM Employees
        WHERE Id = @Id";

        // Execute the delete operation
        await _database.ExecuteAsync(sql, new
        {
            Id = employeeDelete.EmployeeID
        });
    }



    // chatGPT fail

}
