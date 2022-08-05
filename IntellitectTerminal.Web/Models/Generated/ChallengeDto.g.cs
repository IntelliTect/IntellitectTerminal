using IntelliTect.Coalesce;
using IntelliTect.Coalesce.Mapping;
using IntelliTect.Coalesce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IntellitectTerminal.Web.Models
{
    public partial class ChallengeDtoGen : GeneratedDto<IntellitectTerminal.Data.Models.Challenge>
    {
        public ChallengeDtoGen() { }

        private int? _ChallengeId;
        private string _Question;
        private int? _Level;
        private IntellitectTerminal.Data.Models.Challenge.CompilationLanguages? _CompilationLanguage;
        private string _Answer;

        public int? ChallengeId
        {
            get => _ChallengeId;
            set { _ChallengeId = value; Changed(nameof(ChallengeId)); }
        }
        public string Question
        {
            get => _Question;
            set { _Question = value; Changed(nameof(Question)); }
        }
        public int? Level
        {
            get => _Level;
            set { _Level = value; Changed(nameof(Level)); }
        }
        public IntellitectTerminal.Data.Models.Challenge.CompilationLanguages? CompilationLanguage
        {
            get => _CompilationLanguage;
            set { _CompilationLanguage = value; Changed(nameof(CompilationLanguage)); }
        }
        public string Answer
        {
            get => _Answer;
            set { _Answer = value; Changed(nameof(Answer)); }
        }

        /// <summary>
        /// Map from the domain object to the properties of the current DTO instance.
        /// </summary>
        public override void MapFrom(IntellitectTerminal.Data.Models.Challenge obj, IMappingContext context, IncludeTree tree = null)
        {
            if (obj == null) return;
            var includes = context.Includes;

            // Fill the properties of the object.

            this.ChallengeId = obj.ChallengeId;
            this.Question = obj.Question;
            this.Level = obj.Level;
            this.CompilationLanguage = obj.CompilationLanguage;
            this.Answer = obj.Answer;
        }

        /// <summary>
        /// Map from the current DTO instance to the domain object.
        /// </summary>
        public override void MapTo(IntellitectTerminal.Data.Models.Challenge entity, IMappingContext context)
        {
            var includes = context.Includes;

            if (OnUpdate(entity, context)) return;

            if (ShouldMapTo(nameof(ChallengeId))) entity.ChallengeId = (ChallengeId ?? entity.ChallengeId);
            if (ShouldMapTo(nameof(Question))) entity.Question = Question;
            if (ShouldMapTo(nameof(Level))) entity.Level = (Level ?? entity.Level);
            if (ShouldMapTo(nameof(CompilationLanguage))) entity.CompilationLanguage = (CompilationLanguage ?? entity.CompilationLanguage);
            if (ShouldMapTo(nameof(Answer))) entity.Answer = Answer;
        }
    }
}
