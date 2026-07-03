namespace pr.net.Models.Tooling.FetchFileTree;

public class FetchFileTreeFile {

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public List<FetchFileTreeFile> Children { get; set; } = [];

}
