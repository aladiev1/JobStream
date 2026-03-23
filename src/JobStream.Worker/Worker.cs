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
                    .ToList();

                _logger.LogInformation("Found {Count} pending jobs", pendingJobs.Count);

                foreach (var job in pendingJobs)
                {
                    _logger.LogInformation("Processing job {JobId}", job.Id);

                    job.Status = JobStatus.Processing;
                    job.StartedUtc = DateTime.UtcNow;
                    await jobRepository.UpdateAsync(job, stoppingToken);

                    var outputFilePath = await csvExportService.GenerateWeatherExportAsync(job, stoppingToken);

                    job.OutputFilePath = outputFilePath;
                    job.Status = JobStatus.Completed;
                    job.CompletedUtc = DateTime.UtcNow;

                    await jobRepository.UpdateAsync(job, stoppingToken);

                    _logger.LogInformation("Completed job {JobId}", job.Id);
                    _logger.LogInformation("CSV written to {FilePath}", outputFilePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing jobs");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}