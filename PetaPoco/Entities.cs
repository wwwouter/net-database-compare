using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

public interface ITrackable
{
    DateTime CreatedOn { get; set; }
    DateTime? UpdatedOn { get; set; }
}

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

public class Employee : ITrackable
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
    // Simplified for PetaPoco, assuming no lazy loading or EF Core navigation properties
}

public class Project : ITrackable
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

public class Customer : ITrackable
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
    public Point? GeographicLocation { get; set; }
    public int LoyaltyPoints { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public string? Notes { get; set; }
    public string JSONData { get; set; } = "{}";
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

public class ProjectCustomer : ITrackable
{
    public Guid ProjectId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

public class EmployeeHierarchy : ITrackable
{
    public Guid EmployeeId { get; set; }
    public Guid? ManagerId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}
