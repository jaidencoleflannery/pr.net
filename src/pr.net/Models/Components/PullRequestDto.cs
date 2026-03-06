using System.Text.Json.Serialization;

namespace pr.net.Models;

public class PullRequest {

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
   public AccountDto Author { get; set; } = new AccountDto();

   [JsonPropertyName("source")]
   public SourceDto Source { get; set; } = new SourceDto();

   [JsonPropertyName("destination")]
   public DestinationDto Destination { get; set; } = new DestinationDto();

   [JsonPropertyName("merge_commit")]
   public CommitDto MergeCommit { get; set; } = new CommitDto();

   [JsonPropertyName("participants")]
   public List<AccountDto> Participants { get; set; } = new List<AccountDto>();

   [JsonPropertyName("reviewers")]
   public List<AccountDto> Reviewers { get; set; } = new List<AccountDto>();

   [JsonPropertyName("close_source_branch")]
   public bool CloseSourceBranch { get; set; }

   [JsonPropertyName("closed_by")]
   public AccountDto ClosedBy { get; set; } = new AccountDto();

   [JsonPropertyName("reason")]
   public string Reason { get; set; } = string.Empty;

   [JsonPropertyName("created_on")]
   public DateTime CreatedOn { get; set; }

   [JsonPropertyName("updated_on")]
   public DateTime UpdatedOn { get; set; }

   [JsonPropertyName("links")]
   public LinksDto Links { get; set; } = new LinksDto();

}