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
