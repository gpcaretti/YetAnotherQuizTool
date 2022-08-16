using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Quiz.Application.Exams;
using Quiz.Application.Sessions;
using Quiz.Application.Users;
using Quiz.Domain.Exams;
using Quiz.Domain.Exams.Sessions;


namespace Quiz.Application {

    internal class AutomapperProfile : Profile {

        public AutomapperProfile() {
            DefaultMaps();
            CandidateMaps();
            ExamMaps();
            ExamSessionMaps();
            //ExamQuestions();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DefaultMaps() {
            // trim all strings
            CreateMap<string, string>()
                .ConvertUsing(str => (str ?? "").Trim());
        }

        /// <summary>
        /// 
        /// </summary>
        private void CandidateMaps() {
            CreateMap<IdentityUser<Guid>, CandidateDto>()
                ;

            CreateMap<CandidateDto, IdentityUser<Guid>>()
                //.ForMember(dst => dst.CreatedOn, src => src.Ignore())
                ;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ExamMaps() {
            CreateMap<Exam, ExamDto>()
                ;

            CreateMap<ExamDto, Exam>()
                //.ForMember(dst => dst.CreatedOn, src => src.Ignore())
                ;

            CreateMap<Choice, ChoiceDto>()
                ;

            CreateMap<Question, QuestionDto>()
                ;
            CreateMap<Question, QuestionAndChoicesDto>()
                ;

            CreateMap<QuestionDto, Question>()
                ;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ExamSessionMaps() {
            CreateMap<ExamSessionRequestDto, ExamSession>()
                ;

            CreateMap<ExamSession, ExamSessionDto>()
                 .ForMember(outp => outp.CandidateId,
                            opt => opt.MapFrom(ex => string.IsNullOrEmpty(ex.CandidateId) ? (Guid?)null : Guid.Parse(ex.CandidateId)))
                ;
            CreateMap<ExamSession, ExamSessionWithAnswersDto>()
                .IncludeBase<ExamSession, ExamSessionDto>()
                ;

            CreateMap<AnswerDetailsDto, ExamSessionItem>()
                ;

            CreateMap<ExamSession, PrepareExamSessionResponseDto>()
                 .ForMember(outp => outp.ExamCode, opt => opt.MapFrom(es => (es.Exam != null) ? es.Exam.Code : null))
                 .ForMember(outp => outp.Duration, opt => opt.MapFrom(es => (es.Exam != null) ? es.Exam.Duration : 0))
                 .ForMember(outp => outp.RandomizeChoices, opt => opt.MapFrom(es => (es.Exam != null) ? es.Exam.RandomChoicesAllowed : false))
            ;

            CreateMap<Exam, PrepareExamSessionResponseDto>()
                 .ForMember(outp => outp.ExamId, opt => opt.MapFrom(ex => ex.Id))
                 .ForMember(outp => outp.ExamName, opt => opt.MapFrom(ex => ex.Name))
                 .ForMember(outp => outp.ExamCode, opt => opt.MapFrom(ex => ex.Code))
                 .ForMember(outp => outp.RandomizeChoices, opt => opt.MapFrom(ex => ex.RandomChoicesAllowed))
            ;

        }
    }
}
