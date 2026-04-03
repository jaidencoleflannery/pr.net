using System.Text;

using pr.net.Models.Generic;

namespace pr.net.Services.Parsing;

public static class ParserService {

    // split diff per file.
    public static bool ParseDiff(string diff, out List<DiffSection> diffSections) {
        diffSections = [];
        string file = string.Empty;
        var builder = new StringBuilder(); 
        foreach(var line in diff.Split('\n')) {
            if(line.StartsWith("diff --git")) {
                if(!String.IsNullOrWhiteSpace(file) && builder.Length > 0)
                    diffSections.Add(new DiffSection(file, builder.ToString()));
                file = string.Empty;
                builder.Clear();
                foreach(var word in line.Split(' '))
                    if(word.StartsWith("b/"))
                        file = word.Substring(2);
            }
            builder.AppendLine(line);
        }

        if(builder.Length > 0)
            diffSections.Add(new DiffSection(file, builder.ToString()));

        if(diffSections.Count <= 0)
            return false;
        else
            return true;
    }

}