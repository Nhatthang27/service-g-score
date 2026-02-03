namespace GScore.Infrastructure.Settings;

public class PostgresSetting
{
    public const string SectionName = "Postgres";

    public string ConnectionString { get; set; } = string.Empty;
}
