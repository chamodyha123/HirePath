using HirePathAI.API.Configuration;
using HirePathAI.API.Services.EmailTemplates;
using HirePathAI.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace HirePathAI.API.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody)
        {
            try
            {
                using var smtp = new SmtpClient
                {
                    Host = _settings.Host,
                    Port = _settings.Port,
                    EnableSsl = _settings.EnableSSL,
                    Credentials = new NetworkCredential(
                        _settings.Username,
                        _settings.Password)
                };

                using var mail = new MailMessage
                {
                    From = new MailAddress(
                        _settings.SenderEmail,
                        _settings.SenderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("Email sending failed: " + ex.Message);
            }
        }

        public async Task SendOtpEmailAsync(
            string toEmail,
            string otp,
            string purpose)
        {
            var html = EmailTemplateBuilder.BuildOtpEmail(otp, purpose);

            await SendEmailAsync(
                toEmail,
                "HirePath AI Verification Code",
                html);
        }

        public async Task SendWelcomeEmailAsync(
            string toEmail,
            string fullName)
        {
            var html = EmailTemplateBuilder.BuildWelcomeEmail(fullName);

            await SendEmailAsync(
                toEmail,
                "Welcome to HirePath AI",
                html);
        }

        public async Task SendInterviewInvitationAsync(
            string toEmail,
            string candidateName,
            string jobTitle,
            DateTime interviewDateTime,
            string interviewType,
            string? meetingLink)
        {
            var html = EmailTemplateBuilder.BuildInterviewInvitationEmail(
                candidateName,
                jobTitle,
                interviewDateTime,
                interviewType,
                meetingLink);

            await SendEmailAsync(
                toEmail,
                "HirePath AI Interview Invitation",
                html);
        }

        public async Task SendInterviewReminderAsync(
            string toEmail,
            string candidateName,
            string jobTitle,
            DateTime interviewDateTime,
            string? meetingLink)
        {
            var html = EmailTemplateBuilder.BuildInterviewReminderEmail(
                candidateName,
                jobTitle,
                interviewDateTime,
                meetingLink);

            await SendEmailAsync(
                toEmail,
                "HirePath AI Interview Reminder",
                html);
        }

        public async Task SendApplicationStatusEmailAsync(
            string toEmail,
            string candidateName,
            string jobTitle,
            string status,
            string? message)
        {
            var html = EmailTemplateBuilder.BuildApplicationStatusEmail(
                candidateName,
                jobTitle,
                status,
                message);

            await SendEmailAsync(
                toEmail,
                "HirePath AI Application Status Update",
                html);
        }

        public async Task SendCustomEmailAsync(
            string toEmail,
            string subject,
            string title,
            string message)
        {
            var html = EmailTemplateBuilder.BuildCustomEmail(title, message);

            await SendEmailAsync(toEmail, subject, html);
        }
    }
}