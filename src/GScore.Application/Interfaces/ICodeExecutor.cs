namespace GScore.Application.Interfaces;

public interface ICodeExecutor
{
    Task<ExecutionResult> ExecuteAsync(
       string language,
       string sourceCode,
       int timeoutSeconds = 10,
       int memoryLimitMb = 256);
}

public record ExecutionResult(
    bool Success,
    string Stdout,
    string Stderr,
    int? ExitCode,
    int ExecutionTimeMs,
    decimal MemoryUsageMb,
    string? ErrorMessage);
