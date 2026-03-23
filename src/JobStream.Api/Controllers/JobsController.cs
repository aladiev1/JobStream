using JobStream.Api.Contracts.Requests;
using JobStream.Api.Contracts.Responses;
using JobStream.Domain.Entities;
using JobStream.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace JobStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private static readonly List<Job> Jobs = new();

    [HttpPost]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<JobResponse> CreateJob(CreateJobRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Location))
        {
            return BadRequest("Location is required.");
        }

        if (!string.Equals(request.Format, "csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only csv format is currently supported.");
        }

        if (!Enum.TryParse<JobPriority>(request.Priority, true, out var priority))
        {
            return BadRequest("Priority must be Low, Normal, or High.");
        }

        var job = new Job
        {
            Location = request.Location.Trim(),
            Format = request.Format.Trim().ToLowerInvariant(),
            Priority = priority
        };

        Jobs.Add(job);

        var response = MapToResponse(job);

        return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<JobResponse> GetJobById(Guid id)
    {
        var job = Jobs.FirstOrDefault(x => x.Id == id);

        if (job is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(job));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<JobResponse>> GetJobs()
    {
        var response = Jobs
            .OrderByDescending(x => x.CreatedUtc)
            .Select(MapToResponse)
            .ToList();

        return Ok(response);
    }

    private static JobResponse MapToResponse(Job job)
    {
        return new JobResponse
        {
            Id = job.Id,
            Location = job.Location,
            Format = job.Format,
            Status = job.Status.ToString(),
            Priority = job.Priority.ToString(),
            CreatedUtc = job.CreatedUtc,
            StartedUtc = job.StartedUtc,
            CompletedUtc = job.CompletedUtc,
            AttemptCount = job.AttemptCount,
            MaxAttempts = job.MaxAttempts,
            OutputFilePath = job.OutputFilePath,
            LastError = job.LastError
        };
    }
}