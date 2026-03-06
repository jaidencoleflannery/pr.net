using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace pr.net.Services;

public class LocalContextService : IContextService {

    private string _path = Path.Combine(System.AppContext.BaseDirectory, "CLAUDE.md");
    private List<string>? _instructions = new List<string>();
    
    public async Task<List<string>> GetInstructions() {
        if(_instructions != null)
            return _instructions;
        else if(File.Exists(_path))
            return _instructions = [.. await File.ReadAllLinesAsync(_path)];
        else 
            throw new InvalidOperationException("Could not read CLAUDE.md for instructions.");
    }
}