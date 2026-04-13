namespace pr.net.Services.Chat.Instructions;

public interface IInstructionsService {

    Task<List<string>> GetInstructions(bool isForFiltering);
    
}