namespace GScore.Presentation.Commons;

public sealed record ApiResponse<T>
{
    public required bool Success { get; init; }
    public T? Data { get; init; }
    public ApiError? Error { get; init; }

    public static ApiResponse<T> SuccessResponse(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Error = null
        };
    }

    public static ApiResponse<T> ErrorResponse(ApiError error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Error = error
        };
    }
}