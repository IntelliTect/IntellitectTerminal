
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
    [Route("api/UserService")]
    [ServiceFilter(typeof(IApiActionFilter))]
    public partial class UserServiceController : Controller
    {
        protected IntellitectTerminal.Data.Services.IUserService Service { get; }

        public UserServiceController(IntellitectTerminal.Data.Services.IUserService service)
        {
            Service = service;
        }

        /// <summary>
        /// Method: InitializeFileSystem
        /// </summary>
        [HttpPost("InitializeFileSystem")]
        [Authorize]
        public virtual ItemResult<UserDtoGen> InitializeFileSystem(UserDtoGen user)
        {
            IncludeTree includeTree = null;
            var _mappingContext = new MappingContext(User);
            var _methodResult = Service.InitializeFileSystem(user.MapToModel(new IntellitectTerminal.Data.Models.User(), _mappingContext));
            var _result = new ItemResult<UserDtoGen>();
            _result.Object = Mapper.MapToDto<IntellitectTerminal.Data.Models.User, UserDtoGen>(_methodResult, _mappingContext, includeTree);
            return _result;
        }
    }
}
