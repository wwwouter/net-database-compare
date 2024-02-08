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
            IsActive = true // IsActive should be true upon creation
        };

        using (var transaction = _session.BeginTransaction())
        {
            await _session.SaveAsync(employee);
            await transaction.CommitAsync();
        }
    }

    public async Task UpdateEmployeeName(EmployeeUpdateNameDto employeeUpdate)
    {
        using (var transaction = _session.BeginTransaction())
        {
            var employee = await _session.Query<Employee>()
                                         .Where(e => e.Id == employeeUpdate.EmployeeID)
                                         .FirstOrDefaultAsync();

            if (employee != null)
            {
                employee.Name = employeeUpdate.Name;
                // Set UpdatedOn to the current UTC date and time
                employee.UpdatedOn = DateTime.UtcNow;

                await _session.SaveOrUpdateAsync(employee);
                await transaction.CommitAsync();
            }
            //          // HQL query to update employee name directly
            // var hql = "update Employee set Name = :newName where Id = :employeeId";
            // await _session.CreateQuery(hql)
            //               .SetParameter("newName", employeeUpdate.Name)
            //               .SetParameter("employeeId", employeeUpdate.EmployeeID)
            //               .ExecuteUpdateAsync();
        }
    }

}
