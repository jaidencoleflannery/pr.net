using System.Text;
using pr.net.Models;

namespace pr.net.Services;

public static class ParserService {

    // split diff per file
    public static List<string> ParseDiff(string diff) {
        var diffSection = new List<string>();
        var builder = new StringBuilder();
        foreach(var line in diff.Split('\n')) {
            if(line.StartsWith("diff --git")) {
                diffSection.Add(builder.ToString());
                builder.Clear();
            }
            builder.AppendLine(line);
        }

        if(builder.Length > 0)
            diffSection.Add(builder.ToString());

        return diffSection;
    }
}