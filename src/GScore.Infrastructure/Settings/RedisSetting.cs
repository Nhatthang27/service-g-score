namespace GScore.Infrastructure.Settings;

public class RedisSetting
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = "livecode";
}
