
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
    [Route("api/User")]
    [Authorize]
    [ServiceFilter(typeof(IApiActionFilter))]
    public partial class UserController
        : BaseApiController<IntellitectTerminal.Data.Models.User, UserDtoGen, IntellitectTerminal.Data.AppDbContext>
    {
        public UserController(IntellitectTerminal.Data.AppDbContext db) : base(db)
        {
            GeneratedForClassViewModel = ReflectionRepository.Global.GetClassViewModel<IntellitectTerminal.Data.Models.User>();
        }

        [HttpGet("get/{id}")]
        [Authorize]
        public virtual Task<ItemResult<UserDtoGen>> Get(
            System.Guid id,
            DataSourceParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.User> dataSource)
            => GetImplementation(id, parameters, dataSource);

        [HttpGet("list")]
        [Authorize]
        public virtual Task<ListResult<UserDtoGen>> List(
            ListParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.User> dataSource)
            => ListImplementation(parameters, dataSource);

        [HttpGet("count")]
        [Authorize]
        public virtual Task<ItemResult<int>> Count(
            FilterParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.User> dataSource)
            => CountImplementation(parameters, dataSource);

        [HttpPost("save")]
        [Authorize]
        public virtual Task<ItemResult<UserDtoGen>> Save(
            UserDtoGen dto,
            [FromQuery] DataSourceParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.User> dataSource,
            IBehaviors<IntellitectTerminal.Data.Models.User> behaviors)
            => SaveImplementation(dto, parameters, dataSource, behaviors);

        [HttpPost("delete/{id}")]
        [Authorize]
        public virtual Task<ItemResult<UserDtoGen>> Delete(
            System.Guid id,
            IBehaviors<IntellitectTerminal.Data.Models.User> behaviors,
            IDataSource<IntellitectTerminal.Data.Models.User> dataSource)
            => DeleteImplementation(id, new DataSourceParameters(), dataSource, behaviors);
    }
}
