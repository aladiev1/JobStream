namespace JobStream.Api.Contracts.Responses;

public class JobResponse
{
    public Guid Id { get; set; }

    public string Location { get; set; } = string.Empty;

    public string Format { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; }

    public DateTime? StartedUtc { get; set; }

    public DateTime? CompletedUtc { get; set; }

    public int AttemptCount { get; set; }

    public int MaxAttempts { get; set; }

    public string? OutputFilePath { get; set; }

    public string? LastError { get; set; }

    public DateTime UpdatedUtc { get; set; }
}