using IntelliTect.Coalesce;
using IntelliTect.Coalesce.Mapping;
using IntelliTect.Coalesce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IntellitectTerminal.Web.Models
{
    public partial class SubmissionDtoGen : GeneratedDto<IntellitectTerminal.Data.Models.Submission>
    {
        public SubmissionDtoGen() { }

        private int? _SubmissionId;
        private IntellitectTerminal.Web.Models.UserDtoGen _User;
        private IntellitectTerminal.Web.Models.ChallengeDtoGen _Challenge;
        private bool? _IsCorrect;
        private string _Content;

        public int? SubmissionId
        {
            get => _SubmissionId;
            set { _SubmissionId = value; Changed(nameof(SubmissionId)); }
        }
        public IntellitectTerminal.Web.Models.UserDtoGen User
        {
            get => _User;
            set { _User = value; Changed(nameof(User)); }
        }
        public IntellitectTerminal.Web.Models.ChallengeDtoGen Challenge
        {
            get => _Challenge;
            set { _Challenge = value; Changed(nameof(Challenge)); }
        }
        public bool? IsCorrect
        {
            get => _IsCorrect;
            set { _IsCorrect = value; Changed(nameof(IsCorrect)); }
        }
        public string Content
        {
            get => _Content;
            set { _Content = value; Changed(nameof(Content)); }
        }

        /// <summary>
        /// Map from the domain object to the properties of the current DTO instance.
        /// </summary>
        public override void MapFrom(IntellitectTerminal.Data.Models.Submission obj, IMappingContext context, IncludeTree tree = null)
        {
            if (obj == null) return;
            var includes = context.Includes;

            // Fill the properties of the object.

            this.SubmissionId = obj.SubmissionId;
            this.IsCorrect = obj.IsCorrect;
            this.Content = obj.Content;
            if (tree == null || tree[nameof(this.User)] != null)
                this.User = obj.User.MapToDto<IntellitectTerminal.Data.Models.User, UserDtoGen>(context, tree?[nameof(this.User)]);

            if (tree == null || tree[nameof(this.Challenge)] != null)
                this.Challenge = obj.Challenge.MapToDto<IntellitectTerminal.Data.Models.Challenge, ChallengeDtoGen>(context, tree?[nameof(this.Challenge)]);

        }

        /// <summary>
        /// Map from the current DTO instance to the domain object.
        /// </summary>
        public override void MapTo(IntellitectTerminal.Data.Models.Submission entity, IMappingContext context)
        {
            var includes = context.Includes;

            if (OnUpdate(entity, context)) return;

            if (ShouldMapTo(nameof(SubmissionId))) entity.SubmissionId = (SubmissionId ?? entity.SubmissionId);
            if (ShouldMapTo(nameof(IsCorrect))) entity.IsCorrect = IsCorrect;
            if (ShouldMapTo(nameof(Content))) entity.Content = Content;
        }
    }
}
