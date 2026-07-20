using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces;

public interface IApplicationRepository
    : IGenericRepository<JobApplication>
{
    Task<IEnumerable<JobApplication>> GetCandidateApplications(int candidateId);

    Task<IEnumerable<JobApplication>> GetJobApplications(int jobId);

    // Loads Job -> Company so services can verify company ownership
    Task<JobApplication?> GetByIdWithDetailsAsync(int id);

    // All applications belonging to jobs owned by the given company —
    // this is what Recruiters/Hiring Managers are scoped to.
    Task<IEnumerable<JobApplication>> GetByCompanyAsync(int companyId);
}