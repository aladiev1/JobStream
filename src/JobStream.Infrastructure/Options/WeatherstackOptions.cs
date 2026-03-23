namespace JobStream.Infrastructure.Options;

public class WeatherstackOptions
{
    public const string SectionName = "Weatherstack";

    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
}