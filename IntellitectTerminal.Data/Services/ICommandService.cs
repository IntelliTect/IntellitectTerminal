using IntelliTect.Coalesce;
using IntelliTect.Coalesce.DataAnnotations;
using IntelliTect.Coalesce.Models;
using IntellitectTerminal.Data.Models;

namespace IntellitectTerminal.Data.Services;

[Coalesce, Service]
public interface ICommandService
{
    [Execute(PermissionLevel = SecurityPermissionLevels.AllowAll)]
    string Request(Guid? userId);
    [Execute(PermissionLevel = SecurityPermissionLevels.AllowAll)]
    string Cat(Guid userId, string fileName);
    [Execute(PermissionLevel = SecurityPermissionLevels.AllowAll)]
    public string Progress(Guid userId);
    [Execute(PermissionLevel = SecurityPermissionLevels.AllowAll)]
    public bool Verify(Guid userId);
    [Execute(PermissionLevel = SecurityPermissionLevels.AllowAll)]
    public Task SubmitFile(IFile file, Guid userId);
    [Execute(PermissionLevel = SecurityPermissionLevels.AllowAll)]
    public Task SubmitUserInput(string input, Guid userId);
}
