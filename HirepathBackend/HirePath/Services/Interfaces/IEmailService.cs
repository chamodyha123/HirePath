namespace HirePathAI.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);

        Task SendOtpEmailAsync(string toEmail, string otp, string purpose);

        Task SendWelcomeEmailAsync(string toEmail, string fullName);

        Task SendInterviewInvitationAsync(
            string toEmail,
            string candidateName,
            string jobTitle,
            DateTime interviewDateTime,
            string interviewType,
            string? meetingLink);

        Task SendInterviewReminderAsync(
            string toEmail,
            string candidateName,
            string jobTitle,
            DateTime interviewDateTime,
            string? meetingLink);

        Task SendApplicationStatusEmailAsync(
            string toEmail,
            string candidateName,
            string jobTitle,
            string status,
            string? message);

        Task SendCustomEmailAsync(
            string toEmail,
            string subject,
            string title,
            string message);
    }
}