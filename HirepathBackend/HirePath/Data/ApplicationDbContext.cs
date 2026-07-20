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

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<CandidateProfile> CandidateProfiles => Set<CandidateProfile>();
        public DbSet<CandidateSkill> CandidateSkills => Set<CandidateSkill>();
        public DbSet<CandidateEducation> CandidateEducations => Set<CandidateEducation>();
        public DbSet<CandidateExperience> CandidateExperiences => Set<CandidateExperience>();
        public DbSet<Resume> Resumes => Set<Resume>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobSkill> JobSkills => Set<JobSkill>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<Interview> Interviews => Set<Interview>();
        public DbSet<InterviewFeedback> InterviewFeedbacks => Set<InterviewFeedback>();
        public DbSet<Evaluation> Evaluations => Set<Evaluation>();
        public DbSet<ApplicationStatusHistory> ApplicationStatusHistories => Set<ApplicationStatusHistory>();

        public DbSet<EmailOtp> EmailOtps => Set<EmailOtp>();
        public DbSet<PendingRegistration>
PendingRegistrations
=> Set<PendingRegistration>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // USER
            // =========================
            modelBuilder.Entity<User>()
                .HasIndex(u => u.NormalizedEmail)
                .HasDatabaseName("IX_User_NormalizedEmail");


            modelBuilder.Entity<EmailOtp>()
    .Property(e => e.Email)
    .HasMaxLength(150);

            modelBuilder.Entity<EmailOtp>()
                .Property(e => e.OtpHash)
                .HasMaxLength(256);

            modelBuilder.Entity<EmailOtp>()
                .HasIndex(e => new
                {
                    e.Email,
                    e.Purpose
                });



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


            modelBuilder.Entity<JobApplication>()
                .Property(j => j.MatchScore)
                .HasPrecision(5, 2);

            // =========================
            // CANDIDATE PROFILE -> EDUCATIONS
            // =========================
            modelBuilder.Entity<CandidateEducation>()
                .HasOne(e => e.CandidateProfile)
                .WithMany(cp => cp.Educations)
                .HasForeignKey(e => e.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // =========================
            // JOB APPLICATIONS -> RESUME
            // =========================
            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Resume)
                .WithMany()
                .HasForeignKey(ja => ja.ResumeId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // INTERVIEWS
            // =========================
            modelBuilder.Entity<Interview>()
                .HasOne(i => i.JobApplication)
                .WithMany(ja => ja.Interviews)
                .HasForeignKey(i => i.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Interview>()
                .HasOne(i => i.ScheduledByUser)
                .WithMany()
                .HasForeignKey(i => i.ScheduledByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // USER -> COMPANY (Recruiter / Hiring Manager membership)
            // =========================
            modelBuilder.Entity<User>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // INTERVIEW FEEDBACK
            // =========================
            modelBuilder.Entity<InterviewFeedback>()
                .HasOne(f => f.Interview)
                .WithMany()
                .HasForeignKey(f => f.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InterviewFeedback>()
                .HasOne(f => f.SubmittedByUser)
                .WithMany()
                .HasForeignKey(f => f.SubmittedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // EVALUATION (1:1 with JobApplication)
            // =========================
            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.JobApplication)
                .WithOne(ja => ja.Evaluation)
                .HasForeignKey<Evaluation>(e => e.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.EvaluatedByUser)
                .WithMany()
                .HasForeignKey(e => e.EvaluatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.ResumeScore).HasPrecision(5, 2);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.AIScore).HasPrecision(5, 2);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.InterviewScore).HasPrecision(5, 2);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.OverallScore).HasPrecision(5, 2);

            // =========================
            // APPLICATION STATUS HISTORY (audit trail)
            // =========================
            modelBuilder.Entity<ApplicationStatusHistory>()
                .HasOne(h => h.JobApplication)
                .WithMany(ja => ja.StatusHistory)
                .HasForeignKey(h => h.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationStatusHistory>()
                .HasOne(h => h.ChangedByUser)
                .WithMany()
                .HasForeignKey(h => h.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // DECIMAL PRECISION
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