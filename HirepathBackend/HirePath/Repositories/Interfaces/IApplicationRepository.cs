// HirePathAI.API/Repositories/Interfaces/IApplicationRepository.cs
using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces;

public interface IApplicationRepository
    : IGenericRepository<JobApplication>
{
    Task<IEnumerable<JobApplication>> GetCandidateApplications(int candidateId);
    Task<IEnumerable<JobApplication>> GetJobApplications(int jobId);
    Task<JobApplication?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<JobApplication>> GetByCompanyAsync(int companyId);
    // Remove the line below if it exists:
    // Task<bool> HasAppliedAsync(int candidateId, int jobId);
}