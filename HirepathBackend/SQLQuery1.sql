SELECT Id, UserId, Headline FROM CandidateProfiles WHERE UserId = 5;
INSERT INTO Resumes (CandidateProfileId, FileName, FilePath, IsPrimary, CreatedAt, UpdatedAt)
VALUES (2, 'cara_resume.pdf', 'https://example.com/resumes/cara_resume.pdf', 1, GETUTCDATE(), NULL);