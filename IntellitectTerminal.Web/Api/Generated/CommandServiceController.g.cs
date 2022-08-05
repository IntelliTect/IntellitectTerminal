
using IntelliTect.Coalesce;
using IntelliTect.Coalesce.Api;
using IntelliTect.Coalesce.Api.Controllers;
using IntelliTect.Coalesce.Api.DataSources;
using IntelliTect.Coalesce.Mapping;
using IntelliTect.Coalesce.Mapping.IncludeTrees;
using IntelliTect.Coalesce.Models;
using IntelliTect.Coalesce.TypeDefinition;
using IntellitectTerminal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IntellitectTerminal.Web.Api
{
    [Route("api/CommandService")]
    [ServiceFilter(typeof(IApiActionFilter))]
    public partial class CommandServiceController : Controller
    {
        protected IntellitectTerminal.Web.ICommandService Service { get; }

        public CommandServiceController(IntellitectTerminal.Web.ICommandService service)
        {
            Service = service;
        }

        /// <summary>
        /// Method: RequestCommand
        /// </summary>
        [HttpPost("RequestCommand")]
        [Authorize]
        public virtual ItemResult<ChallengeDtoGen> RequestCommand(System.Guid? userId)
        {
            IncludeTree includeTree = null;
            var _mappingContext = new MappingContext(User);
            var _methodResult = Service.RequestCommand(userId);
            var _result = new ItemResult<ChallengeDtoGen>();
            _result.Object = Mapper.MapToDto<IntellitectTerminal.Data.Models.Challenge, ChallengeDtoGen>(_methodResult, _mappingContext, includeTree);
            return _result;
        }
    }
}
