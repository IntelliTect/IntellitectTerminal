using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntellitectTerminal.Data.Models;

#nullable disable

public class Challenge
{
    public int ChallengeId { get; set; }

    [Column(TypeName = "nvarchar(MAX)")]
    [MaxLength(int.MaxValue)]
    public string Question { get; set; }

    [Column(TypeName = "nvarchar(MAX)")]
    [MaxLength(int.MaxValue)]
    public string? Answer { get; set; }

    public int Level { get; set; }

    public CompilationLanguages CompilationLanguage { get; set; }

    public enum CompilationLanguages
    {
        None,
    }
#nullable restore
}
