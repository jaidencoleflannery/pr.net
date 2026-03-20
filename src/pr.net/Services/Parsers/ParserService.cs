using System.Text;
using pr.net.Models;

namespace pr.net.Services;

public static class ParserService {

    // get path from diff
    public static string ParsePathFromDiff(string diff) {
        foreach(var line in diff.Split('\n')) {
            if(line.StartsWith("+++ b")) {
                foreach(var word in line.Split(' ')) {
                    if(word.StartsWith("b/")) {
                        return line.Replace("b", "");
                    }
                }
            }
        }
        throw new FormatException("Could not parse path from provided diff.");
    }

    // split diff per file
    public static Dictionary<string, string> ParseDiff(string diff) {
        var diffSections = new Dictionary<string, string>();
        string file = string.Empty;
        var builder = new StringBuilder(); 
        foreach(var line in diff.Split('\n')) {
            if(line.StartsWith("diff --git")) {
                foreach(var word in line.Split(' ')) {
                    if(word.StartsWith(" b/")) {
                        file = word;
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