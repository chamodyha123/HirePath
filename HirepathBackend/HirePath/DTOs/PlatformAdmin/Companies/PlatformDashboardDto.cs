namespace HirePathAI.API.DTOs.PlatformAdmin.Dashboard
{
    public class PlatformDashboardDto
    {
        public int TotalCompanies { get; set; }

        public int PendingCompanies { get; set; }

        public int ApprovedCompanies { get; set; }

        public int RejectedCompanies { get; set; }

        public int SuspendedCompanies { get; set; }

        public int TotalUsers { get; set; }

        public int TotalJobs { get; set; }

        public int TotalApplications { get; set; }
    }
}