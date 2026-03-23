using System.Text.Json.Serialization;

namespace JobStream.Infrastructure.Contracts;

public class WeatherstackCurrentResponse
{
    [JsonPropertyName("location")]
    public WeatherstackLocation? Location { get; set; }

    [JsonPropertyName("current")]
    public WeatherstackCurrent? Current { get; set; }
}

public class WeatherstackLocation
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class WeatherstackCurrent
{
    [JsonPropertyName("observation_time")]
    public string? ObservationTime { get; set; }

    [JsonPropertyName("temperature")]
    public decimal Temperature { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("wind_speed")]
    public decimal WindSpeed { get; set; }

    [JsonPropertyName("weather_descriptions")]
    public List<string>? WeatherDescriptions { get; set; }
}