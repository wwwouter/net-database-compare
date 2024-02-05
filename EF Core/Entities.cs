using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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


[Table("Employees")]
public class Employee
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public int? Age { get; set; }

    public string Department { get; set; }

    public DateTime HireDate { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal? Salary { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string City { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool IsActive { get; set; }

    // Navigation properties
    public virtual ICollection<Project> AssignedProjects { get; set; } = new List<Project>();
    public virtual EmployeeHierarchy? EmployeeHierarchy { get; set; }
}





[Table("Projects")]
public class Project
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(19, 4)")]
    public decimal Budget { get; set; }

    public ProjectStatus Status { get; set; }

    public byte[] LogoSvg { get; set; }

    public string? Notes { get; set; }

    public float Progress { get; set; }

    public ProjectPriority Priority { get; set; }

    public Guid? EmployeeAssigned { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    // Navigation properties
    [ForeignKey("EmployeeAssigned")]
    public virtual Employee? Employee { get; set; }
}






[Table("Customers")]
public class Customer
{
    [Key]
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

    [Column(TypeName = "nvarchar(max)")]
    public string JSONData { get; set; } = "{}";

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }
}






[Table("ProjectCustomers")]
public class ProjectCustomer
{
    [Key, Column(Order = 0)]
    public Guid ProjectId { get; set; }

    [Key, Column(Order = 1)]
    public Guid CustomerId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }



    // Navigation properties
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
}





[Table("EmployeeHierarchy")]
public class EmployeeHierarchy
{
    [Key]
    public Guid EmployeeId { get; set; }

    public Guid? ManagerId { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    // Navigation properties
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }

    [ForeignKey("ManagerId")]
    public virtual Employee? Manager { get; set; }
}
