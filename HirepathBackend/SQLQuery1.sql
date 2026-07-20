<<<<<<< HEAD
﻿SELECT Id, UserId, Headline FROM CandidateProfiles WHERE UserId = 5;
INSERT INTO Resumes (CandidateProfileId, FileName, FilePath, IsPrimary, CreatedAt, UpdatedAt)
VALUES (2, 'cara_resume.pdf', 'https://example.com/resumes/cara_resume.pdf', 1, GETUTCDATE(), NULL);
=======
﻿SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'CandidateEducations' 
AND COLUMN_NAME = 'Grade';
>>>>>>> c1f8599e77879f19fe016d9afdcfa72093156b15
