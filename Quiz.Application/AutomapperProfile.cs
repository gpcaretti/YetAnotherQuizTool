using AutoMapper;
using PatenteN.Quiz.Domain.Exams;
using PatenteN.Quiz.Domain.Users;

namespace PatenteN.Quiz.Application {

    internal class AutomapperProfile : Profile {

        public AutomapperProfile() {
            DefaultMaps();
            CandidateMaps();
            ExamMaps();
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
            CreateMap<Candidate, CandidateDto>()
                ;

            CreateMap<CandidateDto, Candidate>()
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
    }
}
