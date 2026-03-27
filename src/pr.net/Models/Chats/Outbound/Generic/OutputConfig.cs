using System.Text.Json.Serialization;
using pr.net.Models.Outbound.Anthropic;

namespace pr.net.Models.Outbound.Generic;

// all added instances of outputconfigdto have to be added here in order for OutputConfig to properly serialize
[JsonDerivedType(typeof(AnthropicOutputConfig))]
[JsonDerivedType(typeof(AnthropicFilteringOutputConfigDto))]
public abstract class OutputConfig { }