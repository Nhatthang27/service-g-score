using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GScore.Domain.Constants;

public enum ExecutionStatus
{
    Queued,
    Running,
    Completed,
    Failed,
    Timeout
}