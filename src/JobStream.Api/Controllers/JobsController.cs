using JobStream.Api.Contracts.Requests;
using JobStream.Api.Contracts.Responses;
using JobStream.Application.Interfaces;
using JobStream.Domain.Entities;
using JobStream.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace JobStream.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobRepository _jobRepository;
    private readonly IFileService _fileService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobRepository jobRepository, IFileService fileService, ILogger<JobsController> logger)
    {
        _jobRepository = jobRepository;
        _fileService = fileService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobResponse>> CreateJob(CreateJobRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Location))
        {
            return BadRequest("Location is required.");
        }

        var format = request.Format?.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(format) || format != "csv")
        {
            return BadRequest("Only csv format is currently supported.");
        }

        if (!Enum.TryParse<JobPriority>(request.Priority, true, out var priority))
        {
            return BadRequest("Priority must be Low, Normal, or High.");
        }

        var job = new Job
        {
            Id = Guid.NewGuid(),
            Location = request.Location.Trim(),
            Format = format!,
            Priority = priority,
            Status = JobStatus.Pending,
            AttemptCount = 0,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        await _jobRepository.AddAsync(job, cancellationToken);

        var response = MapToResponse(job);

        _logger.LogInformation(
            "Created job {JobId} for location {Location} with priority {Priority}",
            job.Id,
            job.Location,
            job.Priority);

        return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobResponse>> GetJobById(Guid id, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(id, cancellationToken);

        if (job is null)
        {
            _logger.LogWarning("Job {JobId} not found", id);
            return NotFound();
        }

        _logger.LogInformation("Fetching job {JobId}", id);

        return Ok(MapToResponse(job));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<JobResponse>>> GetJobs(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var jobs = await _jobRepository.GetAllAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<JobStatus>(status, true, out var parsedStatus))
            {
                return BadRequest("Status must be Pending, Processing, Completed, or Failed.");
            }

            jobs = jobs.Where(x => x.Status == parsedStatus).ToList();
        }

        var response = jobs
            .OrderByDescending(x => x.CreatedUtc)
            .Select(MapToResponse)
            .ToList();

        if (!string.IsNullOrWhiteSpace(status))
        {
            _logger.LogInformation("Fetching jobs with status filter: {Status}", status);
        }

        return Ok(response);
    }

    [HttpGet("{id:guid}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DownloadJobFile(Guid id, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(id, cancellationToken);

        if (job is null)
        {
            _logger.LogWarning("Job {JobId} not found", id);
            return NotFound();
        }

        if (job.Status == JobStatus.Failed)
        {
            return BadRequest("This job failed and does not have a downloadable file.");
        }

        if (job.Status != JobStatus.Completed)
        {
            return BadRequest($"This job is not ready for download. Current status: {job.Status}.");
        }

        if (string.IsNullOrWhiteSpace(job.OutputFilePath))
        {
            return BadRequest("This job does not have an output file.");
        }

        if (!_fileService.FileExists(job.OutputFilePath))
        {
            return NotFound("The output file could not be found.");
        }

        var stream = _fileService.OpenRead(job.OutputFilePath);
        var fileName = _fileService.GetFileName(job.OutputFilePath);

        _logger.LogInformation("Downloading file for job {JobId}: {FilePath}", job.Id, job.OutputFilePath);

        return File(stream, "text/csv", fileName);
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
            LastError = job.LastError,
            UpdatedUtc = job.UpdatedUtc
        };
    }
}