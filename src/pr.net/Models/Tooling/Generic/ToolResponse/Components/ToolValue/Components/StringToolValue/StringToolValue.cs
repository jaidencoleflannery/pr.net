namespace pr.net.Models.Tooling;

public class StringToolValue : ToolValue {

    public ICollection<string> Value { get; init; }

    public StringToolValue(ICollection<string> value) {
        this.Value = value;
    }

}

