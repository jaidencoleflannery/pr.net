using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Tooling;

public class ToolingConfiguration {

    public bool Enabled { get; private set; }
 
    [Required]
    [ConfigurationKeyName("Enabled")] 
    public string? EnabledString { get; init { 
        bool.TryParse(value, out bool parseResult); 
        Enabled = parseResult; 
        field = value; 
    }}

}

