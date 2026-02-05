namespace GScore.Application.Exceptions.Errors;

public static class CsvFileErrors
{
    public static readonly Error InvalidCsvFile = new Error(
        "INVALID_CSV_FILE",
        "The uploaded file is not a valid CSV file.");

    public static readonly Error EmptyCsvFile = new Error(
        "EMPTY_CSV_FILE",
        "The uploaded CSV file is empty.");
}
