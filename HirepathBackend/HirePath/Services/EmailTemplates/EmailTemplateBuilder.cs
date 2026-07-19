namespace HirePathAI.API.Services.EmailTemplates
{
    public static class EmailTemplateBuilder
    {
        private static string Layout(string title, string content)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
</head>
<body style='margin:0;padding:0;background:#f1f5f9;font-family:Arial,sans-serif;'>
    <div style='max-width:620px;margin:30px auto;background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 12px 35px rgba(15,23,42,.15);'>
        <div style='background:linear-gradient(135deg,#06111f,#2563eb,#7c3aed);padding:28px;text-align:center;color:white;'>
            <h1 style='margin:0;font-size:28px;'>HirePath AI</h1>
            <p style='margin:8px 0 0;color:#dbeafe;'>AI Powered Recruitment Platform</p>
        </div>

        <div style='padding:32px;color:#1e293b;'>
            <h2 style='margin-top:0;color:#0f172a;'>{title}</h2>
            {content}
        </div>

        <div style='padding:18px 32px;background:#f8fafc;color:#64748b;font-size:13px;text-align:center;'>
            <p style='margin:0;'>This is an automated email from HirePath AI.</p>
            <p style='margin:6px 0 0;'>Please do not reply to this message.</p>
        </div>
    </div>
</body>
</html>";
        }

        public static string BuildOtpEmail(string otp, string purpose)
        {
            var content = $@"
<p style='font-size:16px;line-height:1.7;'>
    Your {purpose} verification code is:
</p>

<div style='text-align:center;margin:28px 0;'>
    <div style='display:inline-block;background:#eff6ff;color:#2563eb;font-size:38px;font-weight:800;letter-spacing:10px;padding:18px 28px;border-radius:14px;border:1px solid #bfdbfe;'>
        {otp}
    </div>
</div>

<p style='font-size:15px;line-height:1.7;color:#475569;'>
    This code will expire in <strong>5 minutes</strong>. Do not share this code with anyone.
</p>";

            return Layout("Verification Code", content);
        }

        public static string BuildWelcomeEmail(string fullName)
        {
            var content = $@"
<p style='font-size:16px;line-height:1.7;'>
    Hello <strong>{fullName}</strong>,
</p>

<p style='font-size:16px;line-height:1.7;'>
    Welcome to <strong>HirePath AI</strong>. Your account has been created successfully.
</p>

<p style='font-size:16px;line-height:1.7;color:#475569;'>
    You can now explore jobs, manage your profile, apply for opportunities, and use AI-powered recruitment features.
</p>";

            return Layout("Welcome to HirePath AI", content);
        }

        public static string BuildInterviewInvitationEmail(
            string candidateName,
            string jobTitle,
            DateTime interviewDateTime,
            string interviewType,
            string? meetingLink)
        {
            var linkHtml = string.IsNullOrWhiteSpace(meetingLink)
                ? "<p style='color:#64748b;'>Meeting link will be shared later.</p>"
                : $@"<p><a href='{meetingLink}' style='display:inline-block;background:#2563eb;color:white;text-decoration:none;padding:12px 20px;border-radius:10px;font-weight:bold;'>Join Interview</a></p>";

            var content = $@"
<p style='font-size:16px;line-height:1.7;'>Hello <strong>{candidateName}</strong>,</p>

<p style='font-size:16px;line-height:1.7;'>
    You have been invited for an interview for the position:
</p>

<div style='background:#f8fafc;border:1px solid #e2e8f0;border-radius:12px;padding:18px;margin:20px 0;'>
    <p><strong>Job Title:</strong> {jobTitle}</p>
    <p><strong>Date & Time:</strong> {interviewDateTime:dddd, dd MMMM yyyy hh:mm tt}</p>
    <p><strong>Interview Type:</strong> {interviewType}</p>
</div>

{linkHtml}

<p style='color:#475569;'>Please be available on time and prepare accordingly.</p>";

            return Layout("Interview Invitation", content);
        }

        public static string BuildInterviewReminderEmail(
            string candidateName,
            string jobTitle,
            DateTime interviewDateTime,
            string? meetingLink)
        {
            var linkHtml = string.IsNullOrWhiteSpace(meetingLink)
                ? ""
                : $@"<p><a href='{meetingLink}' style='display:inline-block;background:#7c3aed;color:white;text-decoration:none;padding:12px 20px;border-radius:10px;font-weight:bold;'>Open Meeting Link</a></p>";

            var content = $@"
<p style='font-size:16px;line-height:1.7;'>Hello <strong>{candidateName}</strong>,</p>

<p style='font-size:16px;line-height:1.7;'>
    This is a reminder for your upcoming interview.
</p>

<div style='background:#f8fafc;border:1px solid #e2e8f0;border-radius:12px;padding:18px;margin:20px 0;'>
    <p><strong>Job Title:</strong> {jobTitle}</p>
    <p><strong>Date & Time:</strong> {interviewDateTime:dddd, dd MMMM yyyy hh:mm tt}</p>
</div>

{linkHtml}

<p style='color:#475569;'>Good luck with your interview.</p>";

            return Layout("Interview Reminder", content);
        }

        public static string BuildApplicationStatusEmail(
            string candidateName,
            string jobTitle,
            string status,
            string? message)
        {
            var content = $@"
<p style='font-size:16px;line-height:1.7;'>Hello <strong>{candidateName}</strong>,</p>

<p style='font-size:16px;line-height:1.7;'>
    Your application status for <strong>{jobTitle}</strong> has been updated.
</p>

<div style='background:#eff6ff;border:1px solid #bfdbfe;border-radius:12px;padding:18px;margin:20px 0;text-align:center;'>
    <p style='margin:0;color:#2563eb;font-size:22px;font-weight:800;'>{status}</p>
</div>

<p style='font-size:15px;line-height:1.7;color:#475569;'>
    {message ?? "Please check your HirePath AI dashboard for more details."}
</p>";

            return Layout("Application Status Update", content);
        }

        public static string BuildCustomEmail(string title, string message)
        {
            var content = $@"
<p style='font-size:16px;line-height:1.7;color:#475569;'>
    {message}
</p>";

            return Layout(title, content);
        }
    }
}