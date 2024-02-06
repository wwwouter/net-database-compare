using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EmployeeProjectRepository : IEmployeeProjectRepository
{
    private readonly IDataContext _db;

    public EmployeeProjectRepository(IDataContext db)
    {
        _db = db;
    }

    public async Task AddEmployee(EmployeeAddDto employeeDto)
    {
        // Convert EmployeeAddDto to Employee entity
        var employee = new Employee
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
            IsActive = true // Assuming IsActive is set to true by default
        };

        // Using IDataContext for database operations
        using (var db = (DataConnection)_db)
        {
            await db.InsertAsync(employee);
        }
    }

    public async Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate)
    {
        // Using IDataContext for database operations
        using (var db = (DataConnection)_db)
        {
            // Retrieve the employee by ID
            var employee = await db.GetTable<Employee>()
                                   .FirstOrDefaultAsync(e => e.Id == employeeUpdate.EmployeeID);

            if (employee != null)
            {
                // Update the employee's name
                employee.Name = employeeUpdate.Name;

                // Set UpdatedOn to the current UTC time
                employee.UpdatedOn = DateTime.UtcNow;

                // Update the employee entity in the database
                await db.UpdateAsync(employee);
            }
        }
    }

    public async Task DeleteEmployeeById(EmployeeDeleteDto employeeDelete)
    {
        // Using IDataContext for database operations
        using (var db = (DataConnection)_db)
        {
            // Attempt to delete the employee by ID directly
            // This approach assumes the ID column and the parameter are directly matched
            var deletedCount = await db.GetTable<Employee>()
                                       .Where(e => e.Id == employeeDelete.EmployeeID)
                                       .DeleteAsync();

            if (deletedCount == 0)
            {
                // Handle the case where the employee does not exist or could not be deleted
                // This might include logging the event or throwing an exception
                // depending on how you wish to handle this scenario
            }
        }
    }




}
