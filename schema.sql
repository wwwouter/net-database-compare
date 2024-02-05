CREATE TABLE Employees (
    Id uniqueidentifier NOT NULL PRIMARY KEY DEFAULT newid(),
    Name nvarchar(50) NOT NULL,
    Age int,
    Department nvarchar(20) NOT NULL,
    HireDate datetime2 NOT NULL,
    Salary decimal(19,4),
    AddressLine1 nvarchar(50),
    AddressLine2 nvarchar(50),
    City nvarchar(30),
    CreatedOn datetime2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedOn datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1
);

CREATE TABLE Projects (
    Id uniqueidentifier NOT NULL PRIMARY KEY DEFAULT newid(),
    Name nvarchar(50) NOT NULL,
    StartDate datetime2 NOT NULL,
    EndDate datetime2 NOT NULL,
    Budget decimal(19,4) NOT NULL,
    Status tinyint NOT NULL,
    LogoSvg varbinary(MAX) NOT NULL,
    Notes nvarchar(MAX),
    Progress float NOT NULL,
    Priority tinyint NOT NULL,
    EmployeeAssigned uniqueidentifier,
    FOREIGN KEY (EmployeeAssigned) REFERENCES Employees(Id),
    CreatedOn datetime2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedOn datetime2 NULL
);

CREATE TABLE Customers (
    Id uniqueidentifier NOT NULL PRIMARY KEY DEFAULT newid(),
    Name nvarchar(50) NOT NULL,
    Age int,
    Email nvarchar(254) NOT NULL,
    PhoneNumber nvarchar(15),
    AddressLine1 nvarchar(50),
    AddressLine2 nvarchar(50),
    City nvarchar(30) NOT NULL,
    Country nvarchar(25) NOT NULL,
    GeographicLocation geography,
    LoyaltyPoints int NOT NULL,
    LastPurchaseDate datetime2,
    Notes nvarchar(MAX),
    JSONData nvarchar(max) NOT NULL DEFAULT '{}',
    CreatedOn datetime2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedOn datetime2 NULL
);

CREATE TABLE ProjectCustomers (
    ProjectId uniqueidentifier NOT NULL,
    CustomerId uniqueidentifier NOT NULL,
    StartDate date NOT NULL,
    EndDate date NULL,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id),
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    PRIMARY KEY (ProjectId, CustomerId),
    CreatedOn datetime2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedOn datetime2 NULL
);

CREATE TABLE EmployeeHierarchy (
    EmployeeId uniqueidentifier NOT NULL,
    ManagerId uniqueidentifier NULL,
    FOREIGN KEY (EmployeeId) REFERENCES Employee(Id),
    FOREIGN KEY (ManagerId) REFERENCES Employee(Id),
    PRIMARY KEY (EmployeeId),
    CreatedOn datetime2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedOn datetime2 NULL
);

CREATE VIEW ProjectSummaries AS
SELECT
    p.Id AS ProjectID,
    p.Name,
    p.Budget AS TotalBudget,
    p.Status,
    p.StartDate,
    p.EndDate,
    p.Progress,
    p.Priority,
    e.Name AS EmployeeAssignedName,
    (SELECT COUNT(*) FROM ProjectCustomers WHERE ProjectId = p.Id) AS NumberOfCustomers
FROM
    Projects p
LEFT JOIN Employees e ON p.EmployeeAssigned = e.Id;


CREATE PROCEDURE GetEmployeesByDepartment
    @Department NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id AS EmployeeID,
        Name,
        Age,
        Department,
        HireDate,
        Salary,
        AddressLine1,
        AddressLine2,
        City
    FROM Employees
    WHERE Department = @Department
END


CREATE TABLE AppliedMigrations (
    MigrationId INT IDENTITY(1,1) PRIMARY KEY,
    ScriptName VARCHAR(255) NOT NULL,
    AppliedBy VARCHAR(255) NOT NULL,
    AppliedAt DATETIME NOT NULL
);


CREATE TABLE MigrationLock (
    LockId INT PRIMARY KEY,
    IsLocked BIT NOT NULL,
    LockedBy VARCHAR(255),
    LockAcquiredAt DATETIME
);


