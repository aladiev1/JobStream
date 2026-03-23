using JobStream.Domain.Entities;

namespace JobStream.Application.Interfaces;

public interface ICsvExportService
{
    Task<string> GenerateWeatherExportAsync(Job job, CancellationToken cancellationToken = default);
}