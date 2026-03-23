using System.Text;
using JobStream.Application.Interfaces;
using JobStream.Domain.Entities;

namespace JobStream.Infrastructure.Services;

public class CsvExportService : ICsvExportService
{
    public async Task<string> GenerateWeatherExportAsync(Job job, CancellationToken cancellationToken = default)
    {
        var outputDirectory = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "outputs");
        var fullOutputDirectory = Path.GetFullPath(outputDirectory);

        Directory.CreateDirectory(fullOutputDirectory);

        var fileName = $"job-{job.Id}.csv";
        var filePath = Path.Combine(fullOutputDirectory, fileName);

        var rows = new List<string>
        {
            "Location,ObservedUtc,TemperatureF,Humidity,WindMph,Condition",
            $"{Escape(job.Location)},{DateTime.UtcNow:O},58,62,8,Cloudy",
            $"{Escape(job.Location)},{DateTime.UtcNow.AddMinutes(-30):O},57,64,7,Cloudy",
            $"{Escape(job.Location)},{DateTime.UtcNow.AddHours(-1):O},56,66,6,Overcast"
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