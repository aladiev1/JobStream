using System.Text;
using JobStream.Application.Interfaces;
using JobStream.Domain.Entities;

namespace JobStream.Infrastructure.Services;

public class CsvExportService : ICsvExportService
{
    private readonly IWeatherClient _weatherClient;

    public CsvExportService(IWeatherClient weatherClient)
    {
        _weatherClient = weatherClient;
    }

    public async Task<string> GenerateWeatherExportAsync(Job job, CancellationToken cancellationToken = default)
    {
        var snapshot = await _weatherClient.GetCurrentWeatherAsync(job.Location, cancellationToken);

        var outputDirectory = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "outputs");
        var fullOutputDirectory = Path.GetFullPath(outputDirectory);

        Directory.CreateDirectory(fullOutputDirectory);

        var fileName = $"job-{job.Id}.csv";
        var filePath = Path.Combine(fullOutputDirectory, fileName);

        var rows = new List<string>
        {
            "Location,ObservedUtc,Temperature,Humidity,WindSpeed,Description",
            $"{Escape(snapshot.Location)},{snapshot.ObservedUtc:O},{snapshot.Temperature},{snapshot.Humidity},{snapshot.WindSpeed},{Escape(snapshot.Description)}"
        };

        var csv = string.Join(Environment.NewLine, rows);

        await File.WriteAllTextAsync(filePath, csv, Encoding.UTF8, cancellationToken);

        return filePath;
    }

    private static string Escape(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}