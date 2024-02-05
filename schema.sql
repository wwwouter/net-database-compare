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
