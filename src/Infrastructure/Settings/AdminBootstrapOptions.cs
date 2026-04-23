namespace Infrastructure.Settings;

public class AdminBootstrapOptions
{
    public const string SectionName = "AdminBootstrap";

    public bool Enabled { get; init; }

    public string Email { get; init; } = string.Empty;
}
