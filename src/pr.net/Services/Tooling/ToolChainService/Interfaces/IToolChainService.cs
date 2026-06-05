using pr.net.Models.Tooling;

namespace pr.net.Services.Tooling;

public interface IToolChain {

    public IEnumerable<ToolSignature> GetToolSignatures();

}

