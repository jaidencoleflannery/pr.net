namespace pr.net.Services.Chat.Instructions;

public class LocalInstructionsService : IInstructionsService {

    private string _path = Path.Combine(Directory.GetCurrentDirectory(), "Instructions/INSTRUCTIONS.md");
    private string _filteringPath = Path.Combine(Directory.GetCurrentDirectory(), "Instructions/FILTERING_INSTRUCTIONS.md");
    private List<string>? _instructions = new List<string>();
    private List<string>? _filteringInstructions = new List<string>();
    
    // instructions is a list of strings so that we can parse it downstream
    public async Task<List<string>> GetInstructions(bool isForFiltering = false) {
        if(isForFiltering) {
            if(_filteringInstructions?.Count > 0)
                return _filteringInstructions;
            
            if(File.Exists(_filteringPath))
                return _filteringInstructions = [.. await File.ReadAllLinesAsync(_filteringPath)];
            else 
                throw new InvalidOperationException("Could not read FILTERING_INSTRUCTIONS.md for instructions. Please ensure this file is in the root directory of this project.");
        } else {
            if(_instructions?.Count > 0)
                return _instructions; 

            if(File.Exists(_path))
                return _instructions = [.. await File.ReadAllLinesAsync(_path)];
            else 
                throw new InvalidOperationException("Could not read INSTRUCTIONS.md for instructions. Please ensure this file is in the root directory of this project.");
        }
    }

}