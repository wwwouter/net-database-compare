CREATE TABLE Employee (
    EmployeeID uniqueidentifier NOT NULL PRIMARY KEY DEFAULT newid(),
    Name nvarchar(50) NOT NULL,
    Age int NOT NULL,
    Gender tinyint NOT NULL,
    Department nvarchar(20) NOT NULL,
    HireDate datetime2 NOT NULL,
    Salary decimal(19,4) NOT NULL,
    AddressLine1 nvarchar(50),
    AddressLine2 nvarchar(50),
    City nvarchar(30),
    CreatedOn datetime2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedOn datetime2 NULL
);

CREATE TABLE Project (
    ProjectID uniqueidentifier NOT NULL PRIMARY KEY DEFAULT newid(),
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
    FOREIGN KEY (EmployeeAssigned) REFERENCES Employee(EmployeeID),
    CreatedOn datetime2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedOn datetime2 NULL
);

CREATE TABLE Customer (
    CustomerID uniqueidentifier NOT NULL PRIMARY KEY DEFAULT newid(),
    Name nvarchar(50) NOT NULL,
    Age int NOT NULL,
    Gender tinyint NOT NULL,
    Email nvarchar(254) NOT NULL,
    PhoneNumber nvarchar(15) NOT NULL,
    AddressLine1 nvarchar(50),
    AddressLine2 nvarchar(50),
    City nvarchar(30) NOT NULL,
    Country nvarchar(25) NOT NULL,
    GeographicLocation geography NOT NULL,
    LoyaltyPoints int NOT NULL,
    LastPurchaseDate datetime2 NOT NULL,
    FavoriteColor tinyint NOT NULL,
    PreferredProduct tinyint NOT NULL,
    Notes nvarchar(MAX),
    JSONData nvarchar(max) NOT NULL DEFAULT '{}',
    CreatedOn datetime2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedOn datetime2 NULL
);

CREATE TABLE ProjectCustomer (
    ProjectID uniqueidentifier NOT NULL,
    CustomerID uniqueidentifier NOT NULL,
    StartDate date NOT NULL,
    EndDate date NULL,
    FOREIGN KEY (ProjectID) REFERENCES Project(ProjectID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    PRIMARY KEY (ProjectID, CustomerID),
    CreatedOn datetime2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedOn datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1
);

CREATE TABLE EmployeeHierarchy (
    EmployeeID uniqueidentifier NOT NULL,
    ManagerID uniqueidentifier NULL,
    FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID),
    FOREIGN KEY (ManagerID) REFERENCES Employee(EmployeeID),
    PRIMARY KEY (EmployeeID),
    CreatedOn datetime2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedOn datetime2 NULL
);
