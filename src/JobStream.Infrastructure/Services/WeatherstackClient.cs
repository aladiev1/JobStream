using System.Text.Json;
using JobStream.Application.Interfaces;
using JobStream.Infrastructure.Contracts;
using JobStream.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace JobStream.Infrastructure.Services;

public class WeatherstackClient : IWeatherClient
{
    private readonly HttpClient _httpClient;
    private readonly WeatherstackOptions _options;

    public WeatherstackClient(HttpClient httpClient, IOptions<WeatherstackOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<WeatherSnapshotDto> GetCurrentWeatherAsync(string location, CancellationToken cancellationToken = default)
    {
        var url =
            $"{_options.BaseUrl.TrimEnd('/')}/current?access_key={Uri.EscapeDataString(_options.ApiKey)}&query={Uri.EscapeDataString(location)}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var payload = await JsonSerializer.DeserializeAsync<WeatherstackCurrentResponse>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);

        if (payload?.Location is null || payload.Current is null)
        {
            throw new InvalidOperationException("Weatherstack response was missing expected data.");
        }

        return new WeatherSnapshotDto
        {
            Location = payload.Location.Name ?? location,
            ObservedUtc = DateTime.UtcNow,
            Temperature = payload.Current.Temperature,
            Humidity = payload.Current.Humidity,
            WindSpeed = payload.Current.WindSpeed,
            Description = payload.Current.WeatherDescriptions?.FirstOrDefault() ?? "Unknown"
        };
    }
}