using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Tooling;

public class ToolingConfiguration {

    public bool Enabled { get; private set; } = true;

    public string Provider { get; private set; } = string.Empty;

    public int MaxInvocations { get; private set; } = -1;
 
    [Required]
    [ConfigurationKeyName("Enabled")] 
    public string? EnabledString { get; set { 
        if(value == null)
            throw new InvalidOperationException($"\n{DateTime.Now}: Tooling configuration field for Enabled is malformed.");

        bool.TryParse(value, out bool parseResult); 
        Enabled = parseResult; 
        field = value; 
    }}

    [Required]
    [ConfigurationKeyName("Provider")]
    public string? ProviderString { get; set {
        if(value == null)
            throw new InvalidOperationException($"\n{DateTime.Now}: Tooling configuration field for Provider is malformed.");

        Provider = value;
        field = value;
    }} 

    [Required]
    [ConfigurationKeyName("MaxInvocations")]
    public string? MaxInvocationsString { get; set {
        if(value == null)
            throw new InvalidOperationException($"\n{DateTime.Now}: Tooling configuration field for MaxInvocations is malformed.");

        int.TryParse(value, out int parseResult);
        MaxInvocations = parseResult;
        field = value;
    }}
}

