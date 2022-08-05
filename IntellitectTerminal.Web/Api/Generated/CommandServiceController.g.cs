
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
        protected IntellitectTerminal.Data.Services.ICommandService Service { get; }

        public CommandServiceController(IntellitectTerminal.Data.Services.ICommandService service)
        {
            Service = service;
        }

        /// <summary>
        /// Method: Request
        /// </summary>
        [HttpPost("Request")]
        [AllowAnonymous]
        public virtual ItemResult<string> Request(System.Guid? userId)
        {
            var _methodResult = Service.Request(userId);
            var _result = new ItemResult<string>();
            _result.Object = _methodResult;
            return _result;
        }

        /// <summary>
        /// Method: Cat
        /// </summary>
        [HttpPost("Cat")]
        [AllowAnonymous]
        public virtual ItemResult<string> Cat(System.Guid userId, string fileName)
        {
            var _methodResult = Service.Cat(userId, fileName);
            var _result = new ItemResult<string>();
            _result.Object = _methodResult;
            return _result;
        }

        /// <summary>
        /// Method: Progress
        /// </summary>
        [HttpPost("Progress")]
        [AllowAnonymous]
        public virtual ItemResult<string> Progress(System.Guid userId)
        {
            var _methodResult = Service.Progress(userId);
            var _result = new ItemResult<string>();
            _result.Object = _methodResult;
            return _result;
        }

        /// <summary>
        /// Method: Verify
        /// </summary>
        [HttpPost("Verify")]
        [AllowAnonymous]
        public virtual ItemResult<bool> Verify(System.Guid userId)
        {
            var _methodResult = Service.Verify(userId);
            var _result = new ItemResult<bool>();
            _result.Object = _methodResult;
            return _result;
        }

        /// <summary>
        /// Method: SubmitFile
        /// </summary>
        [HttpPost("SubmitFile")]
        [AllowAnonymous]
        public virtual async Task<ItemResult> SubmitFile(Microsoft.AspNetCore.Http.IFormFile file, System.Guid userId)
        {
            await Service.SubmitFile(file == null ? null : new IntelliTect.Coalesce.Models.File { Name = file.FileName, ContentType = file.ContentType, Length = file.Length, Content = file.OpenReadStream() }, userId);
            var _result = new ItemResult();
            return _result;
        }

        /// <summary>
        /// Method: SubmitUserInput
        /// </summary>
        [HttpPost("SubmitUserInput")]
        [AllowAnonymous]
        public virtual async Task<ItemResult> SubmitUserInput(string input, System.Guid userId)
        {
            await Service.SubmitUserInput(input, userId);
            var _result = new ItemResult();
            return _result;
        }
    }
}
