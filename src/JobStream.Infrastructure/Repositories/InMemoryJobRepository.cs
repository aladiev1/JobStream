using JobStream.Application.Interfaces;
using JobStream.Domain.Entities;

namespace JobStream.Infrastructure.Repositories;

public class InMemoryJobRepository : IJobRepository
{
    private static readonly List<Job> _jobs = new();

    public Task<Job> AddAsync(Job job, CancellationToken cancellationToken = default)
    {
        _jobs.Add(job);
        return Task.FromResult(job);
    }

    public Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var job = _jobs.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(job);
    }

    public Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult((IReadOnlyList<Job>)_jobs.OrderByDescending(x => x.CreatedUtc).ToList());
    }
}