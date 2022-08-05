namespace IntellitectTerminal.Data.Models;

#nullable disable

public class Submission
{
    public int SubmissionId { get; set; }

    public User User { get; set; }

    public Challenge Challenge { get; set; }

    public string Content { get; set; }
    public bool IsCorrect { get; set; }

#nullable restore
}
