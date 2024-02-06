using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EmployeeProjectRepository : IEmployeeProjectRepository
{
    private readonly ISession _session;

    public EmployeeProjectRepository(ISession session)
    {
        _session = session;
    }

    public async Task AddEmployee(EmployeeAddDto employeeDto)
    {
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
            IsActive = true // Assuming IsActive should be true upon creation
        };

        // Use asynchronous session methods to interact with the database
        using (var transaction = _session.BeginTransaction())
        {
            await _session.SaveAsync(employee);
            await transaction.CommitAsync();
        }
    }

}
