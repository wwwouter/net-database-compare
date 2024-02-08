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