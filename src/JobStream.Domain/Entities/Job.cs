using JobStream.Domain.Enums;

namespace JobStream.Domain.Entities;

public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Location { get; set; } = string.Empty;

    public string Format { get; set; } = "csv";

    public JobStatus Status { get; set; } = JobStatus.Pending;

    public JobPriority Priority { get; set; } = JobPriority.Normal;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public DateTime? StartedUtc { get; set; }

    public DateTime? CompletedUtc { get; set; }

    public int AttemptCount { get; set; } = 0;

    public int MaxAttempts { get; set; } = 3;

    public string? OutputFilePath { get; set; }

    public string? LastError { get; set; }
}