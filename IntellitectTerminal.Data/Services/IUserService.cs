using IntelliTect.Coalesce;
using IntelliTect.Coalesce.DataAnnotations;
using IntellitectTerminal.Data.Models;

namespace IntellitectTerminal.Data.Services;

[Coalesce, Service]
public interface IUserService
{
    [Execute(PermissionLevel = SecurityPermissionLevels.AllowAll)]
    User InitializeFileSystem(Guid? userId);
}
