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
        // IDENTITY / AUTHENTICATION
        // =========================

        public DbSet<EmailOtp> EmailOtps =>
            Set<EmailOtp>();

        public DbSet<PendingRegistration> PendingRegistrations =>
            Set<PendingRegistration>();

        // =========================
        // COMPANY MODULE
        // =========================

        public DbSet<Company> Companies =>
            Set<Company>();

        public DbSet<Department> Departments =>
            Set<Department>();

        public DbSet<CompanyMember> CompanyMembers =>
            Set<CompanyMember>();

        public DbSet<CompanyInvitation> CompanyInvitations =>
            Set<CompanyInvitation>();

        public DbSet<CompanyRegistrationRequest>
            CompanyRegistrationRequests =>
                Set<CompanyRegistrationRequest>();

        // =========================
        // CANDIDATE MODULE
        // =========================

        public DbSet<CandidateProfile> CandidateProfiles =>
            Set<CandidateProfile>();

        public DbSet<CandidateSkill> CandidateSkills =>
            Set<CandidateSkill>();

        public DbSet<CandidateEducation> CandidateEducations =>
            Set<CandidateEducation>();

        public DbSet<CandidateExperience> CandidateExperiences =>
            Set<CandidateExperience>();

        public DbSet<Resume> Resumes =>
            Set<Resume>();

        public DbSet<ProfilePicture> ProfilePictures =>
            Set<ProfilePicture>();

        // =========================
        // RECRUITMENT MODULE
        // =========================

        public DbSet<Job> Jobs =>
            Set<Job>();

        public DbSet<JobSkill> JobSkills =>
            Set<JobSkill>();

        public DbSet<JobApplication> JobApplications =>
            Set<JobApplication>();

        public DbSet<Interview> Interviews =>
            Set<Interview>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // USER
            // =========================

            modelBuilder.Entity<User>()
                .HasIndex(u => u.NormalizedEmail)
                .HasDatabaseName("IX_User_NormalizedEmail");

            modelBuilder.Entity<User>()
                .Property(u => u.FullName)
                .HasMaxLength(150);

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
            // COMPANY
            // =========================

            modelBuilder.Entity<Company>()
                .Property(c => c.Name)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Company>()
                .Property(c => c.Email)
                .HasMaxLength(150);

            modelBuilder.Entity<Company>()
                .HasIndex(c => c.Email);

            // =========================
            // COMPANY -> DEPARTMENTS
            // =========================

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Company)
                .WithMany(c => c.Departments)
                .HasForeignKey(d => d.CompanyId)
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
            // COMPANY -> MEMBERS
            // =========================

            modelBuilder.Entity<CompanyMember>()
                .HasOne(cm => cm.Company)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // USER -> COMPANY MEMBERSHIP
            // =========================

            modelBuilder.Entity<CompanyMember>()
                .HasOne(cm => cm.User)
                .WithOne(u => u.CompanyMembership)
                .HasForeignKey<CompanyMember>(
                    cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompanyMember>()
                .HasIndex(cm => cm.UserId)
                .IsUnique();

            modelBuilder.Entity<CompanyMember>()
                .HasIndex(cm => new
                {
                    cm.CompanyId,
                    cm.UserId
                })
                .IsUnique();

            // =========================
            // COMPANY -> INVITATIONS
            // =========================

            modelBuilder.Entity<CompanyInvitation>()
                .HasOne(ci => ci.Company)
                .WithMany(c => c.Invitations)
                .HasForeignKey(ci => ci.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompanyInvitation>()
                .HasOne(ci => ci.InvitedByUser)
                .WithMany()
                .HasForeignKey(ci => ci.InvitedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CompanyInvitation>()
                .Property(ci => ci.Email)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<CompanyInvitation>()
                .Property(ci => ci.TokenHash)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder.Entity<CompanyInvitation>()
                .HasIndex(ci => ci.TokenHash)
                .IsUnique();

            modelBuilder.Entity<CompanyInvitation>()
                .HasIndex(ci => new
                {
                    ci.Email,
                    ci.Status
                });

            // =========================
            // COMPANY REGISTRATION
            // =========================

            modelBuilder.Entity<CompanyRegistrationRequest>()
                .Property(cr => cr.CompanyName)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<CompanyRegistrationRequest>()
                .Property(cr => cr.CompanyEmail)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<CompanyRegistrationRequest>()
                .Property(cr => cr.RepresentativeEmail)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<CompanyRegistrationRequest>()
                .HasIndex(cr => cr.CompanyEmail);

            modelBuilder.Entity<CompanyRegistrationRequest>()
                .HasIndex(cr => cr.RepresentativeEmail);

            modelBuilder.Entity<CompanyRegistrationRequest>()
                .HasOne(cr => cr.ReviewedByUser)
                .WithMany()
                .HasForeignKey(cr => cr.ReviewedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CompanyRegistrationRequest>()
                .HasOne(cr => cr.CreatedCompany)
                .WithMany()
                .HasForeignKey(cr => cr.CreatedCompanyId)
                .OnDelete(DeleteBehavior.NoAction);

            // =========================
            // USER -> CANDIDATE PROFILE
            // =========================

            modelBuilder.Entity<CandidateProfile>()
                .HasOne(cp => cp.User)
                .WithOne(u => u.CandidateProfile)
                .HasForeignKey<CandidateProfile>(
                    cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // CANDIDATE PROFILE ->
            // PROFILE PICTURE
            // =========================

            modelBuilder.Entity<CandidateProfile>()
                .HasOne(cp => cp.ProfilePicture)
                .WithOne(pp => pp.CandidateProfile)
                .HasForeignKey<CandidateProfile>(
                    cp => cp.ProfilePictureId)
                .OnDelete(DeleteBehavior.SetNull);

            // =========================
            // CANDIDATE PROFILE -> SKILLS
            // =========================

            modelBuilder.Entity<CandidateSkill>()
                .HasOne(cs => cs.CandidateProfile)
                .WithMany(cp => cp.Skills)
                .HasForeignKey(
                    cs => cs.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CandidateSkill>()
                .HasIndex(cs => new
                {
                    cs.CandidateProfileId,
                    cs.SkillName
                })
                .IsUnique();

            // =========================
            // CANDIDATE PROFILE ->
            // EDUCATIONS
            // =========================

            modelBuilder.Entity<CandidateEducation>()
                .HasOne(e => e.CandidateProfile)
                .WithMany(cp => cp.Educations)
                .HasForeignKey(
                    e => e.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CandidateEducation>()
                .Property(e => e.GPA)
                .HasPrecision(3, 2);

            modelBuilder.Entity<CandidateEducation>()
                .Property(e => e.Percentage)
                .HasPrecision(5, 2);

            // =========================
            // CANDIDATE PROFILE ->
            // EXPERIENCES
            // =========================

            modelBuilder.Entity<CandidateExperience>()
                .HasOne(e => e.CandidateProfile)
                .WithMany(cp => cp.Experiences)
                .HasForeignKey(
                    e => e.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // CANDIDATE PROFILE ->
            // RESUMES
            // =========================

            modelBuilder.Entity<Resume>()
                .HasOne(r => r.CandidateProfile)
                .WithMany(cp => cp.Resumes)
                .HasForeignKey(
                    r => r.CandidateProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // JOB SKILLS
            // =========================

            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.Job)
                .WithMany(j => j.RequiredSkills)
                .HasForeignKey(
                    js => js.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobSkill>()
                .HasIndex(js => new
                {
                    js.JobId,
                    js.SkillName
                })
                .IsUnique();

            // =========================
            // JOB APPLICATION -> JOB
            // =========================

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(
                    ja => ja.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // JOB APPLICATION ->
            // CANDIDATE PROFILE
            // =========================

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.CandidateProfile)
                .WithMany(cp => cp.Applications)
                .HasForeignKey(
                    ja => ja.CandidateProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobApplication>()
                .HasIndex(ja => new
                {
                    ja.JobId,
                    ja.CandidateProfileId
                })
                .IsUnique();

            modelBuilder.Entity<JobApplication>()
                .Property(ja => ja.MatchScore)
                .HasPrecision(5, 2);

            // =========================
            // INTERVIEWS
            // =========================

            modelBuilder.Entity<Interview>()
                .HasOne(i => i.JobApplication)
                .WithMany(ja => ja.Interviews)
                .HasForeignKey(
                    i => i.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

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