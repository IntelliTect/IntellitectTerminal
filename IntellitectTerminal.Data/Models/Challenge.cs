namespace IntellitectTerminal.Data.Models;

#nullable disable

public class Challenge
{
    public int ChallengeId { get; set; }

    public string Question { get; set; }

    public string? Answer { get; set; }

    public int Level { get; set; }

    public CompilationLanguages CompilationLanguage { get; set; }

    public enum CompilationLanguages
    {
        None,
    }
#nullable restore
}
