
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
    [Route("api/Submission")]
    [Authorize]
    [ServiceFilter(typeof(IApiActionFilter))]
    public partial class SubmissionController
        : BaseApiController<IntellitectTerminal.Data.Models.Submission, SubmissionDtoGen, IntellitectTerminal.Data.AppDbContext>
    {
        public SubmissionController(IntellitectTerminal.Data.AppDbContext db) : base(db)
        {
            GeneratedForClassViewModel = ReflectionRepository.Global.GetClassViewModel<IntellitectTerminal.Data.Models.Submission>();
        }

        [HttpGet("get/{id}")]
        [Authorize]
        public virtual Task<ItemResult<SubmissionDtoGen>> Get(
            int id,
            DataSourceParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.Submission> dataSource)
            => GetImplementation(id, parameters, dataSource);

        [HttpGet("list")]
        [Authorize]
        public virtual Task<ListResult<SubmissionDtoGen>> List(
            ListParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.Submission> dataSource)
            => ListImplementation(parameters, dataSource);

        [HttpGet("count")]
        [Authorize]
        public virtual Task<ItemResult<int>> Count(
            FilterParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.Submission> dataSource)
            => CountImplementation(parameters, dataSource);

        [HttpPost("save")]
        [Authorize]
        public virtual Task<ItemResult<SubmissionDtoGen>> Save(
            SubmissionDtoGen dto,
            [FromQuery] DataSourceParameters parameters,
            IDataSource<IntellitectTerminal.Data.Models.Submission> dataSource,
            IBehaviors<IntellitectTerminal.Data.Models.Submission> behaviors)
            => SaveImplementation(dto, parameters, dataSource, behaviors);

        [HttpPost("delete/{id}")]
        [Authorize]
        public virtual Task<ItemResult<SubmissionDtoGen>> Delete(
            int id,
            IBehaviors<IntellitectTerminal.Data.Models.Submission> behaviors,
            IDataSource<IntellitectTerminal.Data.Models.Submission> dataSource)
            => DeleteImplementation(id, new DataSourceParameters(), dataSource, behaviors);
    }
}
