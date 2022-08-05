using IntelliTect.Coalesce;
using IntelliTect.Coalesce.Api;
using IntelliTect.Coalesce.DataAnnotations;
using IntelliTect.Coalesce.Helpers;
using IntelliTect.Coalesce.Models;
using IntelliTect.Coalesce.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace IntellitectTerminal.Data.Models;
#nullable disable

public class Submission
{
    public int SubmissionId { get; set; }

    public User User { get; set; }

    public Challenge Challenge { get; set; }

    public bool? IsCorrect { get; set; }

#nullable restore
    public string? Content { get; set; }
}