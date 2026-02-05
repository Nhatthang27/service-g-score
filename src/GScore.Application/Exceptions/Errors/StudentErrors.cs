namespace GScore.Application.Exceptions.Errors;

public static class StudentErrors
{
    public static readonly Error NotFound = new Error(
        "STUDENT_NOT_FOUND",
        "The specified student was not found.");
}
