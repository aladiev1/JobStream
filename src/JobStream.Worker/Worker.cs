using JobStream.Application.Interfaces;
using JobStream.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace JobStream.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("JobStream Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                var csvExportService = scope.ServiceProvider.GetRequiredService<ICsvExportService>();

                var jobs = await jobRepository.GetAllAsync(stoppingToken);
                _logger.LogInformation("Found {Count} total jobs", jobs.Count);

                var pendingJobs = jobs
                    .Where(x => x.Status == JobStatus.Pending)
                    .OrderByDescending(x => x.Priority)
                    .ThenBy(x => x.CreatedUtc)
                    .ToList();

                _logger.LogInformation("Found {Count} pending jobs", pendingJobs.Count);

                foreach (var job in pendingJobs)
                {
                    try
                    {
                        _logger.LogInformation(
                            "Processing job {JobId} for location {Location}. Attempt {Attempt}/{MaxAttempts}",
                            job.Id,
                            job.Location,
                            job.AttemptCount + 1,
                            job.MaxAttempts);

                        job.Status = JobStatus.Processing;
                        job.StartedUtc ??= DateTime.UtcNow;
                        job.UpdatedUtc = DateTime.UtcNow;
                        job.AttemptCount++;
                        job.LastError = null;

                        await jobRepository.UpdateAsync(job, stoppingToken);

                        var outputFilePath = await csvExportService.GenerateWeatherExportAsync(job, stoppingToken);

                        job.OutputFilePath = outputFilePath;
                        job.Status = JobStatus.Completed;
                        job.CompletedUtc = DateTime.UtcNow;
                        job.UpdatedUtc = DateTime.UtcNow;
                        job.LastError = null;

                        await jobRepository.UpdateAsync(job, stoppingToken);

                        _logger.LogInformation(
                            "Completed job {JobId}. CSV written to {FilePath}",
                            job.Id,
                            outputFilePath);
                    }
                    catch (Exception ex)
                    {
                        job.LastError = ex.Message;
                        job.UpdatedUtc = DateTime.UtcNow;

                        if (job.AttemptCount >= job.MaxAttempts)
                        {
                            job.Status = JobStatus.Failed;
                            job.CompletedUtc = DateTime.UtcNow;

                            _logger.LogError(
                                ex,
                                "Job {JobId} failed permanently after {AttemptCount} attempts",
                                job.Id,
                                job.AttemptCount);
                        }
                        else
                        {
                            job.Status = JobStatus.Pending;

                            _logger.LogWarning(
                                ex,
                                "Job {JobId} failed on attempt {AttemptCount}. It will be retried.",
                                job.Id,
                                job.AttemptCount);
                        }

                        await jobRepository.UpdateAsync(job, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in worker execution loop");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}