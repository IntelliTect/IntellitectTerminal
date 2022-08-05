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

public class UploadedFile
{
    public int FileId { get; set; }

    [Read]
    public string AttachmentName { get; set; }

    [Read]
    public long AttachmentSize { get; set; }

    [Read]
    public string AttachmentType { get; set; }

    [Read, MaxLength(32)] // Adjust max length based on chosen hash algorithm.
    public byte[] AttachmentHash { get; set; } // Could also be a base64 string if so desired.

    [InternalUse]
    public FileAttachmentContent AttachmentContent { get; set; } = new();

    [Coalesce]
    public async Task UploadAttachment(AppDbContext db, IFile file)
    {
        if (file.Content == null) return;

        var content = new byte[file.Length];
        await file.Content.ReadAsync(content.AsMemory());

        AttachmentContent = new() { FileId = FileId, Content = content };
        AttachmentName = file.Name;
        AttachmentSize = file.Length;
        AttachmentType = file.ContentType;
        AttachmentHash = SHA256.HashData(content);
    }

    [Coalesce]
    [ControllerAction(IntelliTect.Coalesce.DataAnnotations.HttpMethod.Get, VaryByProperty = nameof(AttachmentHash))]
    public IFile DownloadAttachment(AppDbContext db)
    {
        return new IntelliTect.Coalesce.Models.File(db.UploadedFiles
            .Where(c => c.FileId == this.FileId)
            .Select(c => c.AttachmentContent.Content)
        )
        {
            Name = AttachmentName,
            ContentType = AttachmentType,
        };
    }
}

public class FileAttachmentContent
{
    public int FileId { get; set; }

    [Required]
    public byte[] Content { get; set; }
}

