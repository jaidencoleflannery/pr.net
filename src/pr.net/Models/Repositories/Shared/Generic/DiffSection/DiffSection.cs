namespace pr.net.Models.Generic;

public class DiffSection {

    public DiffSection(string path, string contents) {
        this.Path = path;
        this.Contents = contents;
    }

    public string Path { get; set; } = string.Empty;

    public string Contents { get; set; } = string.Empty;

}