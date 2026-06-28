using HirePathAI.API.Models.Entities;

namespace HirePathAI.API.Repositories.Interfaces;

public interface IApplicationRepository
    : IGenericRepository<JobApplication>
{
    Task<IEnumerable<JobApplication>> GetCandidateApplications(int candidateId);

    Task<IEnumerable<JobApplication>> GetJobApplications(int jobId);
}