# dapper

- https://github.com/DapperLib/Dapper.Contrib
    - T Get<T>(id);
    - IEnumerable<T> GetAll<T>();
    - int Insert<T>(T obj);
    - int Insert<T>(Enumerable<T> list);
    - bool Update<T>(T obj);
        - with updatedOn
    - bool Update<T>(Enumerable<T> list);
    - bool Delete<T>(T obj);
    - bool Delete<T>(Enumerable<T> list);
    - bool DeleteAll<T>(); 
    - T Get<T,W>(string table, W where);       
        - in repo wrap in a method with table set.

# general

- Use nameof instead of string
    - var insertEmployeeQuery = $"INSERT INTO Employees(Name, Age, Department) VALUES(@{nameof(employeeDto.Name)},  @{nameof(employeeDto.Age)}, @{nameof(employeeDto.Department)})";

## Defaults in records for default database values

public record Employee(
    Guid Id = default, // Assigns Guid.Empty as the default value
    string Name = "Unknown", // Default name
    int Age = 0, // Default age
    string Department = "None", // Default department
    DateTime CreatedOn = default, // Assigns DateTime.MinValue as default
    DateTime UpdatedOn = default, // Assigns DateTime.MinValue as default
    bool IsActive = true // Default to true
);


## Simple LINQ

`QueryAsync<Employee>(e => e.City == cityQuery.City);`

on Query<TableClass, TSelect>(TSelect select, Expression<Func<TableClass, bool>> where)
or select in param? neh, you need an input class
params Expression<Func<T, object>>[] propertySelectors

only the basic stuff..