using IntelliTect.Coalesce;
using IntellitectTerminal.Data.Models;

namespace IntellitectTerminal.Data.Services;

[Coalesce, Service]
public interface IUserService
{
    User InitializeFileSystem(Guid? userId);
}
