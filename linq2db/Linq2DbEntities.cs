using System;
using LinqToDB.Mapping;
using NetTopologySuite.Geometries;

public enum ProjectStatus
{
    Planned,
    InProgress,
    Completed,
    OnHold
}

public enum ProjectPriority
{
    Low,
    Medium,
    High,
    Critical
}

[Table(Name = "Employees")]
public class Employee
{
    [PrimaryKey, Identity]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column, NotNull]
    public string Name { get; set; }

    [Column, Nullable]
    public int? Age { get; set; }

    [Column, NotNull]
    public string Department { get; set; }

    [Column, NotNull]
    public DateTime HireDate { get; set; }

    [Column(DataType = DataType.Decimal, Precision = 19, Scale = 4), Nullable]
    public decimal? Salary { get; set; }

    [Column, Nullable]
    public string? AddressLine1 { get; set; }

    [Column, Nullable]
    public string? AddressLine2 { get; set; }

    [Column, NotNull]
    public string City { get; set; }

    [Column, NotNull]
    public DateTime CreatedOn { get; set; }

    [Column, Nullable]
    public DateTime? UpdatedOn { get; set; }

    [Column, NotNull]
    public bool IsActive { get; set; }
}

[Table(Name = "Projects")]
public class Project
{
    [PrimaryKey, Identity]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column, NotNull]
    public string Name { get; set; }

    [Column, NotNull]
    public DateTime StartDate { get; set; }

    [Column, NotNull]
    public DateTime EndDate { get; set; }

    [Column(DataType = DataType.Decimal, Precision = 19, Scale = 4), NotNull]
    public decimal Budget { get; set; }

    [Column, NotNull]
    public ProjectStatus Status { get; set; }

    [Column(DataType = DataType.Binary), NotNull]
    public byte[] LogoSvg { get; set; }

    [Column(DataType = DataType.NVarChar, Length = int.MaxValue), Nullable]
    public string? Notes { get; set; }

    [Column, NotNull]
    public float Progress { get; set; }

    [Column, NotNull]
    public ProjectPriority Priority { get; set; }

    [Column, Nullable]
    public Guid? EmployeeAssigned { get; set; }
}

[Table(Name = "Customers")]
public class Customer
{
    [PrimaryKey, Identity]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column, NotNull]
    public string Name { get; set; }

    [Column, Nullable]
    public int? Age { get; set; }

    [Column, NotNull]
    public string Email { get; set; }

    [Column, Nullable]
    public string? PhoneNumber { get; set; }

    [Column, Nullable]
    public string? AddressLine1 { get; set; }

    [Column, Nullable]
    public string? AddressLine2 { get; set; }

    [Column, NotNull]
    public string City { get; set; }

    [Column, NotNull]
    public string Country { get; set; }

    // For geometric types, ensure the database and Linq2DB support them. 
    // You might need to handle these types specifically based on the database being used.
    [Column, Nullable]
    public Point? GeographicLocation { get; set; }

    [Column, NotNull]
    public int LoyaltyPoints { get; set; }

    [Column, Nullable]
    public DateTime? LastPurchaseDate { get; set; }

    [Column(DataType = DataType.NVarChar, Length = int.MaxValue), Nullable]
    public string? Notes { get; set; }

    [Column(DataType = DataType.NVarChar, Length = int.MaxValue), NotNull]
    public string JSONData { get; set; } = "{}";
}

[Table(Name = "ProjectCustomers")]
public class ProjectCustomer
{
    [PrimaryKey(1), NotNull]
    public Guid ProjectId { get; set; }

    [PrimaryKey(2), NotNull]
    public Guid CustomerId { get; set; }
}

[Table(Name = "EmployeeHierarchy")]
public class EmployeeHierarchy
{
    [PrimaryKey, NotNull]
    public Guid EmployeeId { get; set; }

    [Column, Nullable]
    public Guid? ManagerId { get; set; }
}
