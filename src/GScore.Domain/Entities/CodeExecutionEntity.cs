using GScore.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GScore.Domain.Entities;

public class CodeExecutionEntity : BaseEntity<Guid>
{
    public Guid SessionId { get; set; }
    public required string Language { get; set; }
    public required string SourceCode { get; set; }
    public ExecutionStatus Status { get; set; }
    public string Stdout { get; set; } = string.Empty;
    public string Stderr { get; set; } = string.Empty;
    public int? ExecutionTimeMs { get; set; }
    public decimal? MemoryUsageMb { get; set; }
    public int? ExitCode { get; set; }
    public string? ErrorMessage { get; set; }
    public required DateTime QueuedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RetryCount { get; set; } = 0;
    public CodeSessionEntity? Session { get; set; }
}