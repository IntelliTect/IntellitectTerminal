using IntelliTect.Coalesce;
using IntelliTect.Coalesce.DataAnnotations;
using IntellitectTerminal.Data.Models;

namespace IntellitectTerminal.Data.Services;

[Coalesce, Service]
public interface ICommandService
{
    [Execute(PermissionLevel = SecurityPermissionLevels.AllowAll)]
    Challenge Request(Guid? userId);
}
