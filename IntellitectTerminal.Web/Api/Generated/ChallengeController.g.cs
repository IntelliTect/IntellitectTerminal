
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
    [Route("api/Challenge")]
    [Authorize]
    [ServiceFilter(typeof(IApiActionFilter))]
    public partial class ChallengeController
        : BaseApiController<IntellitectTerminal.Data.Models.Challenge, ChallengeDtoGen, IntellitectTerminal.Data.AppDbContext>
    {
        public ChallengeController(IntellitectTerminal.Data.AppDbContext db) : base(db)
        {
            GeneratedForClassViewModel = ReflectionRepository.Global.GetClassViewModel<IntellitectTerminal.Data.Models.Challenge>();
        }

        [HttpGet("get/{id}")]
        [Authorize]
        public virtual Task<ItemResult<ChallengeDtoGen>> Get(
            int id,
            DataSourceParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.Challenge> dataSource)
            => GetImplementation(id, parameters, dataSource);

        [HttpGet("list")]
        [Authorize]
        public virtual Task<ListResult<ChallengeDtoGen>> List(
            ListParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.Challenge> dataSource)
            => ListImplementation(parameters, dataSource);

        [HttpGet("count")]
        [Authorize]
        public virtual Task<ItemResult<int>> Count(
            FilterParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.Challenge> dataSource)
            => CountImplementation(parameters, dataSource);

        [HttpPost("save")]
        [Authorize]
        public virtual Task<ItemResult<ChallengeDtoGen>> Save(
            ChallengeDtoGen dto,
            [FromQuery] DataSourceParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.Challenge> dataSource,
            IBehaviors<IntellitectTerminal.Data.Models.Challenge> behaviors)
            => SaveImplementation(dto, parameters, dataSource, behaviors);

        [HttpPost("delete/{id}")]
        [Authorize]
        public virtual Task<ItemResult<ChallengeDtoGen>> Delete(
            int id,
            IBehaviors<IntellitectTerminal.Data.Models.Challenge> behaviors,
            IDataSource<IntellitectTerminal.Data.Models.Challenge> dataSource)
            => DeleteImplementation(id, new DataSourceParameters(), dataSource, behaviors);
    }
}
