﻿using IntelliTect.Coalesce;
using IntellitectTerminal.Data.Models;

namespace IntellitectTerminal.Data.Services;

[Coalesce, Service]
public interface ICommandService
{
    Challenge Request(Guid? userId);
}
