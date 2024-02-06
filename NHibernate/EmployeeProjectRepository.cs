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

}
