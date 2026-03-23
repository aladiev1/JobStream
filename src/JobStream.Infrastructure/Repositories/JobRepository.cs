using JobStream.Application.Interfaces;
using JobStream.Domain.Entities;
using JobStream.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobStream.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly AppDbContext _dbContext;

    public JobRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Job> AddAsync(Job job, CancellationToken cancellationToken = default)
    {
        _dbContext.Jobs.Add(job);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return job;
    }

    public async Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Jobs
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Job job, CancellationToken cancellationToken = default)
    {
        _dbContext.Jobs.Update(job);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}