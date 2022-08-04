using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntellitectTerminal.Data.Models;

#nullable disable

public class User
{
    public Guid UserId { get; set; }

    [Column(TypeName = "nvarchar(MAX)")]
    [MaxLength(int.MaxValue)]
    public string FileSystem { get; set; }

#nullable restore
}
