using RepoDb.Attributes;
using System;

// No need for ITrackable interface or DataAnnotations attributes
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

// Map your classes to the corresponding table names explicitly if they don't match the class names
[Map("Employees")]
public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public int? Age { get; set; }
    public string Department { get; set; }
    public DateTime HireDate { get; set; }
    public decimal? Salary { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string City { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public bool IsActive { get; set; }
}

[Map("Projects")]
public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }
    public ProjectStatus Status { get; set; }
    public byte[] LogoSvg { get; set; }
    public string? Notes { get; set; }
    public float Progress { get; set; }
    public ProjectPriority Priority { get; set; }
    public Guid? EmployeeAssigned { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

[Map("Customers")]
public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public int? Age { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    // For geographic location, you will need to handle this specifically as RepoDB might not support NetTopologySuite directly
    public string? GeographicLocation { get; set; }
    public int LoyaltyPoints { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public string? Notes { get; set; }
    public string JSONData { get; set; } = "{}";
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

[Map("ProjectCustomers")]
public class ProjectCustomer
{
    public Guid ProjectId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

[Map("EmployeeHierarchy")]
public class EmployeeHierarchy
{
    public Guid EmployeeId { get; set; }
    public Guid? ManagerId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}
