# Candidate module integration fixes

- Candidate endpoints use the authenticated user ID from the JWT.
- Enabled static-file serving for uploaded resumes and profile pictures.
- Resume upload uses multipart fields `File` and `IsPrimary`.
- Run `dotnet ef database update` before testing.
