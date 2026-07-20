INSERT INTO Companies (Name, Description, Website, Location, CreatedAt)
VALUES ('Tech Corp', 'A technology company', 'https://techcorp.com', 'Colombo', GETUTCDATE())

INSERT INTO Jobs (Title, Description, EmploymentType, WorkMode, Location, ExperienceLevel, SalaryMin, SalaryMax, IsActive, CompanyId, CreatedAt)
VALUES ('Software Engineer', 'Looking for a skilled engineer.', 1, 1, 'Colombo', 2, 50000, 80000, 1, 1, GETUTCDATE())