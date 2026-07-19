using HirePathAI.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HirePathAI.API.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =========================
        // EXISTING ENTITIES
        // =========================

        public DbSet<Company>
            Companies
            => Set<Company>();

        public DbSet<Department>
            Departments
            => Set<Department>();

        public DbSet<CandidateProfile>
            CandidateProfiles
            => Set<CandidateProfile>();

        public DbSet<CandidateSkill>
            CandidateSkills
            => Set<CandidateSkill>();

        public DbSet<CandidateEducation>
            CandidateEducations
            => Set<CandidateEducation>();

        public DbSet<CandidateExperience>
            CandidateExperiences
            => Set<CandidateExperience>();

        public DbSet<Resume>
            Resumes
            => Set<Resume>();

        public DbSet<Job>
            Jobs
            => Set<Job>();

        public DbSet<JobSkill>
            JobSkills
            => Set<JobSkill>();

        public DbSet<JobApplication>
            JobApplications
            => Set<JobApplication>();

        public DbSet<Interview>
            Interviews
            => Set<Interview>();

        public DbSet<EmailOtp>
            EmailOtps
            => Set<EmailOtp>();

        public DbSet<PendingRegistration>
            PendingRegistrations
            => Set<PendingRegistration>();


        // =========================
        // PLATFORM ADMIN ENTITIES
        // =========================

        public DbSet<CompanyRegistrationRequest>
            CompanyRegistrationRequests
            => Set<CompanyRegistrationRequest>();

        public DbSet<CompanyInvitation>
            CompanyInvitations
            => Set<CompanyInvitation>();


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =========================
            // USER
            // =========================

            modelBuilder.Entity<User>()
                .HasIndex(u => u.NormalizedEmail)
                .HasDatabaseName(
                    "IX_User_NormalizedEmail");


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
                .OnDelete(
                    DeleteBehavior.Cascade);


            // =========================
            // COMPANY -> JOBS
            // =========================

            modelBuilder.Entity<Job>()
                .HasOne(j => j.Company)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(
                    DeleteBehavior.Restrict);


            // =========================
            // DEPARTMENT -> JOBS
            // =========================

            modelBuilder.Entity<Job>()
                .HasOne(j => j.Department)
                .WithMany(d => d.Jobs)
                .HasForeignKey(j => j.DepartmentId)
                .OnDelete(
                    DeleteBehavior.SetNull);


            // =========================
            // USER -> CANDIDATE PROFILE
            // 1 : 1
            // =========================

            modelBuilder.Entity<CandidateProfile>()
                .HasOne(cp => cp.User)
                .WithOne(u => u.CandidateProfile)
                .HasForeignKey<CandidateProfile>(
                    cp => cp.UserId)
                .OnDelete(
                    DeleteBehavior.Cascade);


            // =========================
            // CANDIDATE PROFILE -> SKILLS
            // =========================

            modelBuilder.Entity<CandidateSkill>()
                .HasOne(cs => cs.CandidateProfile)
                .WithMany(cp => cp.Skills)
                .HasForeignKey(
                    cs => cs.CandidateProfileId)
                .OnDelete(
                    DeleteBehavior.Cascade);

            modelBuilder.Entity<CandidateSkill>()
                .HasIndex(cs => new
                {
                    cs.CandidateProfileId,
                    cs.SkillName
                })
                .IsUnique();


            // =========================
            // CANDIDATE PROFILE -> EDUCATION
            // =========================

            modelBuilder.Entity<CandidateEducation>()
                .HasOne(e => e.CandidateProfile)
                .WithMany(cp => cp.Educations)
                .HasForeignKey(
                    e => e.CandidateProfileId)
                .OnDelete(
                    DeleteBehavior.Cascade);


            // =========================
            // CANDIDATE PROFILE -> EXPERIENCE
            // =========================

            modelBuilder.Entity<CandidateExperience>()
                .HasOne(e => e.CandidateProfile)
                .WithMany(cp => cp.Experiences)
                .HasForeignKey(
                    e => e.CandidateProfileId)
                .OnDelete(
                    DeleteBehavior.Cascade);


            // =========================
            // CANDIDATE PROFILE -> RESUME
            // =========================

            modelBuilder.Entity<Resume>()
                .HasOne(r => r.CandidateProfile)
                .WithMany(cp => cp.Resumes)
                .HasForeignKey(
                    r => r.CandidateProfileId)
                .OnDelete(
                    DeleteBehavior.Cascade);


            // =========================
            // JOB SKILLS
            // =========================

            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.Job)
                .WithMany(j => j.RequiredSkills)
                .HasForeignKey(
                    js => js.JobId)
                .OnDelete(
                    DeleteBehavior.Cascade);

            modelBuilder.Entity<JobSkill>()
                .HasIndex(js => new
                {
                    js.JobId,
                    js.SkillName
                })
                .IsUnique();


            // =========================
            // JOB APPLICATIONS -> JOB
            // =========================

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(
                    ja => ja.JobId)
                .OnDelete(
                    DeleteBehavior.Restrict);


            // =========================
            // JOB APPLICATIONS ->
            // CANDIDATE PROFILE
            // =========================

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.CandidateProfile)
                .WithMany(cp => cp.Applications)
                .HasForeignKey(
                    ja => ja.CandidateProfileId)
                .OnDelete(
                    DeleteBehavior.Restrict);


            // =========================
            // INTERVIEWS
            // =========================

            modelBuilder.Entity<Interview>()
                .HasOne(i => i.JobApplication)
                .WithMany(ja => ja.Interviews)
                .HasForeignKey(
                    i => i.JobApplicationId)
                .OnDelete(
                    DeleteBehavior.Cascade);


            // =========================
            // PLATFORM ADMIN
            // COMPANY REGISTRATION REQUEST
            // =========================

            modelBuilder.Entity<
                CompanyRegistrationRequest>()
                .HasOne(r => r.Company)
                .WithMany()
                .HasForeignKey(
                    r => r.CompanyId)
                .OnDelete(
                    DeleteBehavior.SetNull);


            // =========================
            // PLATFORM ADMIN
            // COMPANY INVITATION
            // =========================

            modelBuilder.Entity<CompanyInvitation>()
                .HasOne(i => i.Company)
                .WithMany()
                .HasForeignKey(
                    i => i.CompanyId)
                .OnDelete(
                    DeleteBehavior.Cascade);

            modelBuilder.Entity<CompanyInvitation>()
                .HasIndex(i => i.TokenHash)
                .IsUnique();

            modelBuilder.Entity<CompanyInvitation>()
                .HasIndex(i => i.Email);


            // =========================
            // DECIMAL PRECISION
            // =========================

            modelBuilder.Entity<JobApplication>()
                .Property(j => j.MatchScore)
                .HasPrecision(5, 2);

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