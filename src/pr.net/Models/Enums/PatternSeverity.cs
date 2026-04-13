namespace pr.net.Models.Enums;

public static class PatternSeverities {
       
    public enum PatternSeverity {
        Black, // security issues, leaks, dangerous code.
        Red, // syntax issues, bad patterns, slow code.
        Yellow, // opinion and style based, ugly code.
        None
    }

}