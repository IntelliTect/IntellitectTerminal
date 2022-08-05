using IntelliTect.Coalesce;
using IntelliTect.Coalesce.DataAnnotations;
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
}
