namespace JobStream.Api.Contracts.Requests;

public class CreateJobRequest
{
    public string Location { get; set; } = string.Empty;

    public string Format { get; set; } = "csv";

    public string Priority { get; set; } = "Normal";
}