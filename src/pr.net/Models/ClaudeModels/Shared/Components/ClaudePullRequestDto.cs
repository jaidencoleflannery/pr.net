using System.Text.Json.Serialization;

namespace pr.net.Models;

public class ClaudePullRequestDto {

    [JsonPropertyName("id")]
   public string Id { get; set; } = string.Empty;

   [JsonPropertyName("title")]
   public string Title { get; set; } = string.Empty;

   [JsonPropertyName("description")]
   public string Description { get; set; } = string.Empty;
   
   [JsonPropertyName("state")]
   public string State { get; set; } = string.Empty;

   [JsonPropertyName("draft")]
   public bool Draft { get; set; }

   [JsonPropertyName("author")]
   public ClaudeAccountDto Author { get; set; } = new ClaudeAccountDto();

   [JsonPropertyName("source")]
   public ClaudeSourceDto Source { get; set; } = new ClaudeSourceDto();

   [JsonPropertyName("destination")]
   public ClaudeDestinationDto Destination { get; set; } = new ClaudeDestinationDto();

   [JsonPropertyName("merge_commit")]
   public ClaudeCommitDto MergeCommit { get; set; } = new ClaudeCommitDto();

   [JsonPropertyName("participants")]
   public List<ClaudeAccountDto> Participants { get; set; } = new List<ClaudeAccountDto>();

   [JsonPropertyName("reviewers")]
   public List<ClaudeAccountDto> Reviewers { get; set; } = new List<ClaudeAccountDto>();

   [JsonPropertyName("close_source_branch")]
   public bool CloseSourceBranch { get; set; }

   [JsonPropertyName("closed_by")]
   public ClaudeAccountDto ClosedBy { get; set; } = new ClaudeAccountDto();

   [JsonPropertyName("reason")]
   public string Reason { get; set; } = string.Empty;

   [JsonPropertyName("created_on")]
   public DateTime CreatedOn { get; set; }

   [JsonPropertyName("updated_on")]
   public DateTime UpdatedOn { get; set; }

   [JsonPropertyName("links")]
   public ClaudeLinksDto Links { get; set; } = new ClaudeLinksDto();

}