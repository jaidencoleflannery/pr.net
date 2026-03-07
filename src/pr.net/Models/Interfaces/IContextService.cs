public interface IContextService {

    // this needs to augment depending on the environment
    // if Development, grab from a root dir instructions.md file
    // if Production, need to add provider functionality and configuration settings -
    // such as options for pulling from an S3 bucket or a different storage system
    Task<List<string>> GetInstructions();
    
}