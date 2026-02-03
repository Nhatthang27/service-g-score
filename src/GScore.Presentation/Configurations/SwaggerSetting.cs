namespace GScore.Presentation.Configurations;

public class SwaggerSetting
{
    public const string SectionName = "Swagger";

    public string Title { get; set; } = "Live Code Execution API";
    public string Version { get; set; } = "v1";
    public string Description { get; set; } = "API for live code execution service";
}
