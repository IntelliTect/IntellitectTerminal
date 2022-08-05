using IntelliTect.Coalesce;
using IntelliTect.Coalesce.DataAnnotations;
using IntelliTect.Coalesce.Models;
using IntellitectTerminal.Data.Models;

namespace IntellitectTerminal.Data.Services;

[Coalesce, Service]
public interface ICommandService
{
    Challenge Request(Guid? userId);
}
