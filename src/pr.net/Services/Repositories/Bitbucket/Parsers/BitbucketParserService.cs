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
                foreach(var word in line.Split(' ')) {
                    if(word.StartsWith("b/")) {
                        file = word.Substring(2);
                        break;
                    }
                }
                diffSections.Add(file, builder.ToString());
                builder.Clear();
            }
            builder.AppendLine(line);
        }

        if(builder.Length > 0)
            diffSections[file] += builder.ToString();

        return diffSections;
    }

}