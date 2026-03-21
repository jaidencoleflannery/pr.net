namespace pr.net.Services.Instructions;

public class LocalInstructionsService : IInstructionsService {

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