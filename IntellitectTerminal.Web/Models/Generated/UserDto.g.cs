using IntelliTect.Coalesce;
using IntelliTect.Coalesce.Mapping;
using IntelliTect.Coalesce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IntellitectTerminal.Web.Models
{
    public partial class UserDtoGen : GeneratedDto<IntellitectTerminal.Data.Models.User>
    {
        public UserDtoGen() { }

        private System.Guid? _UserId;
        private string _FileSystem;
        private System.DateTime? _CreationTime;

        public System.Guid? UserId
        {
            get => _UserId;
            set { _UserId = value; Changed(nameof(UserId)); }
        }
        public string FileSystem
        {
            get => _FileSystem;
            set { _FileSystem = value; Changed(nameof(FileSystem)); }
        }
        public System.DateTime? CreationTime
        {
            get => _CreationTime;
            set { _CreationTime = value; Changed(nameof(CreationTime)); }
        }

        /// <summary>
        /// Map from the domain object to the properties of the current DTO instance.
        /// </summary>
        public override void MapFrom(IntellitectTerminal.Data.Models.User obj, IMappingContext context, IncludeTree tree = null)
        {
            if (obj == null) return;
            var includes = context.Includes;

            // Fill the properties of the object.

            this.UserId = obj.UserId;
            this.FileSystem = obj.FileSystem;
            this.CreationTime = obj.CreationTime;
        }

        /// <summary>
        /// Map from the current DTO instance to the domain object.
        /// </summary>
        public override void MapTo(IntellitectTerminal.Data.Models.User entity, IMappingContext context)
        {
            var includes = context.Includes;

            if (OnUpdate(entity, context)) return;

            if (ShouldMapTo(nameof(UserId))) entity.UserId = (UserId ?? entity.UserId);
            if (ShouldMapTo(nameof(FileSystem))) entity.FileSystem = FileSystem;
            if (ShouldMapTo(nameof(CreationTime))) entity.CreationTime = (CreationTime ?? entity.CreationTime);
        }
    }
}
