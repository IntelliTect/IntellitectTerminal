using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntellitectTerminal.Data.Models;

#nullable disable

public class Submission
{
    public int SubmissionId { get; set; }

    public User User { get; set; }

    public Challenge Challenge { get; set; }

    [Column(TypeName = "nvarchar(MAX)")]
    [MaxLength(int.MaxValue)]
    public string Content { get; set; }

#nullable restore
}
