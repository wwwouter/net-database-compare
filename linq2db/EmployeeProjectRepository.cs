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

}
