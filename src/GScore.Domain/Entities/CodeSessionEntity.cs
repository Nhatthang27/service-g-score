using GScore.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GScore.Domain.Entities;

public class CodeSessionEntity : BaseEntity<Guid>
{
    public required string Language { get; set; }
    public string SourceCode { get; set; } = string.Empty;
    public required SessionStatus Status { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public ICollection<CodeExecutionEntity> Executions { get; set; } = new List<CodeExecutionEntity>();
}