using JobStream.Domain.Entities;

namespace JobStream.Application.Interfaces;

public interface IJobRepository
{
    Task<Job> AddAsync(Job job, CancellationToken cancellationToken = default);

    Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default);
}