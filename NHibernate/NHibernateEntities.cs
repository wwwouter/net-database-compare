public class EmployeeMap : ClassMap<Employee>
{
    public EmployeeMap()
    {
        Table("Employees");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.Name).Not.Nullable().Length(50);
        Map(x => x.Age).Nullable();
        Map(x => x.Department).Not.Nullable().Length(20);
        Map(x => x.HireDate);
        Map(x => x.Salary).Precision(19).Scale(4);
        Map(x => x.AddressLine1);
        Map(x => x.AddressLine2);
        Map(x => x.City);
        Map(x => x.CreatedOn);
        Map(x => x.UpdatedOn);
        Map(x => x.IsActive);
        HasMany(x => x.AssignedProjects).Cascade.All().Inverse().KeyColumn("EmployeeAssigned");
        References(x => x.EmployeeHierarchy).Cascade.All().Column("Id");
    }
}


public class ProjectMap : ClassMap<Project>
{
    public ProjectMap()
    {
        Table("Projects");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.Name).Not.Nullable();
        Map(x => x.StartDate);
        Map(x => x.EndDate);
        Map(x => x.Budget).Precision(19).Scale(4);
        Map(x => x.Status).CustomType<ProjectStatus>();
        Map(x => x.LogoSvg).CustomSqlType("varbinary(max)");
        Map(x => x.Notes);
        Map(x => x.Progress);
        Map(x => x.Priority).CustomType<ProjectPriority>();
        Map(x => x.CreatedOn);
        Map(x => x.UpdatedOn);
        References(x => x.Employee).Column("EmployeeAssigned");
    }
}


public class CustomerMap : ClassMap<Customer>
{
    public CustomerMap()
    {
        Table("Customers");
        Id(x => x.Id).GeneratedBy.GuidComb();
        Map(x => x.Name).Not.Nullable();
        Map(x => x.Age);
        Map(x => x.Email).Not.Nullable();
        Map(x => x.PhoneNumber);
        Map(x => x.AddressLine1);
        Map(x => x.AddressLine2);
        Map(x => x.City);
        Map(x => x.Country);
        Map(x => x.GeographicLocation).CustomType<NetTopologySuite.Geometries.Point>();
        Map(x => x.LoyaltyPoints);
        Map(x => x.LastPurchaseDate);
        Map(x => x.Notes);
        Map(x => x.JSONData).CustomSqlType("nvarchar(max)");
        Map(x => x.CreatedOn);
        Map(x => x.UpdatedOn);
    }
}

public class ProjectCustomerMap : ClassMap<ProjectCustomer>
{
    public ProjectCustomerMap()
    {
        Table("ProjectCustomers");
        CompositeId().KeyReference(x => x.ProjectId, "ProjectId").KeyReference(x => x.CustomerId, "CustomerId");
        Map(x => x.StartDate);
        Map(x => x.EndDate);
        Map(x => x.CreatedOn);
        Map(x => x.UpdatedOn);
        References(x => x.Project).Column("ProjectId");
        References(x => x.Customer).Column("CustomerId");
    }
}


public class EmployeeHierarchyMap : ClassMap<EmployeeHierarchy>
{
    public EmployeeHierarchyMap()
    {
        Table("EmployeeHierarchy");
        Id(x => x.EmployeeId).GeneratedBy.Foreign("Employee");
        HasOne(x => x.Employee).Constrained();
        References(x => x.Manager, "ManagerId");
        Map(x => x.CreatedOn);
        Map(x => x.UpdatedOn);
    }
}
