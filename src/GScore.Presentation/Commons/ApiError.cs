public sealed record ApiError(string Code, string Message, object? Detail = null);
