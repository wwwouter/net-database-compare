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

}
