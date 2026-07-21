using HirePathAI.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ============ EXISTING DBSETS ============
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<CandidateProfile> CandidateProfiles => Set<CandidateProfile>();
        public DbSet<CandidateSkill> CandidateSkills => Set<CandidateSkill>();
        public DbSet<CandidateEducation> CandidateEducations => Set<CandidateEducation>();
        public DbSet<CandidateExperience> CandidateExperiences => Set<CandidateExperience>();
        public DbSet<Resume> Resumes => Set<Resume>();
        public DbSet<ProfilePicture> ProfilePictures => Set<ProfilePicture>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobSkill> JobSkills => Set<JobSkill>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<Interview> Interviews => Set<Interview>();
        public DbSet<EmailOtp> EmailOtps => Set<EmailOtp>();
        public DbSet<PendingRegistration> PendingRegistrations => Set<PendingRegistration>();

        // ============ NEW DBSETS (Member 04) ============
        public DbSet<ApplicationStatusHistory> ApplicationStatusHistories => Set<ApplicationStatusHistory>();
        public DbSet<InterviewFeedback> InterviewFeedbacks => Set<InterviewFeedback>();
        public DbSet<Evaluation> Evaluations => Set<Evaluation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // USER
            // =========================
            modelBuilder.Entity<User>()
                .HasIndex(u => u.NormalizedEmail)
                .HasDatabaseName("IX_User_NormalizedEmail");

            // optional: user -> company (many users may belong to a company)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            // =========================
            // EMAIL OTP
            // =========================
            modelBuilder.Entity<EmailOtp>()
                .Property(e => e.Email)
                .HasMaxLength(150);

            modelBuilder.Entity<EmailOtp>()
                .Property(e => e.OtpHash)
                .HasMaxLength(256);

            modelBuilder.Entity<EmailOtp>()
                .HasIndex(e => new { e.Email, e.Purpose });

            // =========================
            // COMPANY -> DEPARTMENTS
            // =========================
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Company)
                .WithMany(c => c.Departments)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // USER -> CANDIDATE PROFILE (1:1)
            // =========================
            modelBuilder.Entity<CandidateProfile>()
                .HasOne(cp => cp.User)
                .WithOne(u => u.CandidateProfile)
                .HasForeignKey<CandidateProfile>(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // CANDIDATE PROFILE -> PROFILE PICTURE (1:1)
            // =========================
            modelBuilder.Entity<CandidateProfile>()
                .HasOne(cp => cp.ProfilePicture)
                .WithOne(pp => pp.CandidateProfile)
                .HasForeignKey<CandidateProfile>(cp => cp.ProfilePictureId)
                .OnDelete(DeleteBehavior.SetNull);

            // =========================
            // CANDIDATE PROFILE -> SKILLS
            // =========================
            modelBuilder.Entity<CandidateSkill>()
                .HasOne(cs => cs.CandidateProfile)
                .WithMany(cp => cp.Skills)
                .HasForeignKey(cs => cs.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CandidateSkill>()
                .HasIndex(cs => new { cs.CandidateProfileId, cs.SkillName })
                .IsUnique();

            // =========================
            // CANDIDATE PROFILE -> EDUCATIONS
            // =========================
            modelBuilder.Entity<CandidateEducation>()
                .HasOne(e => e.CandidateProfile)
                .WithMany(cp => cp.Educations)
                .HasForeignKey(e => e.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // CANDIDATE EDUCATION - DECIMAL PRECISION
            // =========================
            modelBuilder.Entity<CandidateEducation>()
                .Property(e => e.GPA)
                .HasPrecision(3, 2);

            modelBuilder.Entity<CandidateEducation>()
                .Property(e => e.Percentage)
                .HasPrecision(5, 2);

            // =========================
            // CANDIDATE PROFILE -> EXPERIENCES
            // =========================
            modelBuilder.Entity<CandidateExperience>()
                .HasOne(e => e.CandidateProfile)
                .WithMany(cp => cp.Experiences)
                .HasForeignKey(e => e.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // RESUME
            // =========================
            modelBuilder.Entity<Resume>()
                .HasOne(r => r.CandidateProfile)
                .WithMany(cp => cp.Resumes)
                .HasForeignKey(r => r.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // COMPANY -> JOBS
            // =========================
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Company)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // DEPARTMENT -> JOBS
            // =========================
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Department)
                .WithMany(d => d.Jobs)
                .HasForeignKey(j => j.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // =========================
            // JOB SKILLS
            // =========================
            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.Job)
                .WithMany(j => j.RequiredSkills)
                .HasForeignKey(js => js.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobSkill>()
                .HasIndex(js => new { js.JobId, js.SkillName })
                .IsUnique();

            // =========================
            // JOB APPLICATIONS
            // =========================
            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(ja => ja.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.CandidateProfile)
                .WithMany(cp => cp.Applications)
                .HasForeignKey(ja => ja.CandidateProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobApplication>()
                .Property(j => j.MatchScore)
                .HasPrecision(5, 2);

            // =========================
            // INTERVIEWS
            // =========================
            modelBuilder.Entity<Interview>()
                .HasOne(i => i.JobApplication)
                .WithMany(ja => ja.Interviews)
                .HasForeignKey(i => i.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // APPLICATION STATUS HISTORY (NEW)
            // =========================
            modelBuilder.Entity<ApplicationStatusHistory>()
                .HasOne(ash => ash.Application)
                .WithMany() // No navigation from JobApplication to history to avoid circular reference
                .HasForeignKey(ash => ash.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationStatusHistory>()
                .HasOne(ash => ash.ChangedByUser)
                .WithMany()
                .HasForeignKey(ash => ash.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationStatusHistory>()
                .Property(ash => ash.Status)
                .HasConversion<int>();

            modelBuilder.Entity<ApplicationStatusHistory>()
                .HasIndex(ash => ash.ApplicationId);

            modelBuilder.Entity<ApplicationStatusHistory>()
                .HasIndex(ash => ash.ChangedAt);

            // =========================
            // INTERVIEW FEEDBACK (NEW)
            // =========================
            modelBuilder.Entity<InterviewFeedback>()
                .HasOne(ifb => ifb.Interview)
                .WithMany() // No navigation from Interview to Feedback to avoid circular reference
                .HasForeignKey(ifb => ifb.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InterviewFeedback>()
                .HasOne(ifb => ifb.Application)
                .WithMany() // No navigation from JobApplication to Feedback to avoid circular reference
                .HasForeignKey(ifb => ifb.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InterviewFeedback>()
                .HasOne(ifb => ifb.Evaluator)
                .WithMany()
                .HasForeignKey(ifb => ifb.EvaluatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InterviewFeedback>()
                .Property(ifb => ifb.TechnicalScore)
                .HasPrecision(5, 2);

            modelBuilder.Entity<InterviewFeedback>()
                .Property(ifb => ifb.CommunicationScore)
                .HasPrecision(5, 2);

            modelBuilder.Entity<InterviewFeedback>()
                .Property(ifb => ifb.ProblemSolvingScore)
                .HasPrecision(5, 2);

            modelBuilder.Entity<InterviewFeedback>()
                .Property(ifb => ifb.CulturalFitScore)
                .HasPrecision(5, 2);

            modelBuilder.Entity<InterviewFeedback>()
                .Property(ifb => ifb.OverallScore)
                .HasPrecision(5, 2);

            modelBuilder.Entity<InterviewFeedback>()
                .Property(ifb => ifb.Recommendation)
                .HasConversion<int>();

            modelBuilder.Entity<InterviewFeedback>()
                .HasIndex(ifb => ifb.ApplicationId);

            modelBuilder.Entity<InterviewFeedback>()
                .HasIndex(ifb => ifb.InterviewId)
                .IsUnique(); // One feedback per interview

            // =========================
            // EVALUATION (NEW)
            // =========================
            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Application)
                .WithMany() // No navigation from JobApplication to Evaluation to avoid circular reference
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Evaluator)
                .WithMany()
                .HasForeignKey(e => e.EvaluatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.ResumeScore)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.AIScore)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.InterviewScore)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.HiringManagerScore)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.OverallScore)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Evaluation>()
                .HasIndex(e => e.ApplicationId)
                .IsUnique(); // One evaluation per application

            // =========================
            // DECIMAL PRECISION (EXISTING)
            // =========================
            modelBuilder.Entity<Job>()
                .Property(j => j.SalaryMin)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Job>()
                .Property(j => j.SalaryMax)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Interview>()
                .Property(i => i.Score)
                .HasPrecision(5, 2);
        }
    }
}