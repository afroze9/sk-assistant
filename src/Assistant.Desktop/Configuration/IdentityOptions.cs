namespace Assistant.Desktop.Configuration;

public class IdentityOptions
{
    public required string TenantId { get; set; }

    public required string ClientId { get; set; }

    public required string RedirectUri { get; set; }

    public string[] Scopes { get; set; } = [];
}