using System.Text.Json.Serialization;

namespace pr.net.Models;

public class DestinationDto {

    [JsonPropertyName("branch")]
    public BranchDto Branch { get; set; } = new BranchDto();

    [JsonPropertyName("commit")]
    public CommitDto Commit { get; set; } = new CommitDto();

    [JsonPropertyName("repository")]
    public RepositoryDto Repository { get; set; } = new RepositoryDto();

}