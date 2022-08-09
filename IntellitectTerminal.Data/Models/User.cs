namespace IntellitectTerminal.Data.Models;

#nullable disable

public class User
{
    public Guid UserId { get; set; }

    public string FileSystem { get; set; }
    
    public DateTime CreationTime { get; set; }
#nullable restore
}
