using System.Text;
using pr.net.Models;

namespace pr.net.Services;

public static class BitbucketParserService {

    // split diff per file
    public static Dictionary<string, string> ParseDiff(string diff) {
        var diffSections = new Dictionary<string, string>();
        string file = string.Empty;
        var builder = new StringBuilder(); 
        foreach(var line in diff.Split('\n')) {
            if(line.StartsWith("diff --git")) {
                if(!String.IsNullOrWhiteSpace(file) && builder.Length > 0)
                    diffSections.Add(file, builder.ToString());
                file = string.Empty;
                builder.Clear();
                foreach(var word in line.Split(' '))
                    if(word.StartsWith("b/"))
                        file = word.Substring(2);
            }
            builder.AppendLine(line);
        }
        return diffSections;
    }

}