namespace JobStream.Application.Interfaces;

public interface IWeatherClient
{
    Task<WeatherSnapshotDto> GetCurrentWeatherAsync(string location, CancellationToken cancellationToken = default);
}

public class WeatherSnapshotDto
{
    public string Location { get; set; } = string.Empty;
    public DateTime ObservedUtc { get; set; }
    public decimal Temperature { get; set; }
    public int Humidity { get; set; }
    public decimal WindSpeed { get; set; }
    public string Description { get; set; } = string.Empty;
}