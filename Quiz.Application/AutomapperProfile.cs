using AutoMapper;
using Quiz.Application.Exams;
using Quiz.Application.Exams.Sessions;
using Quiz.Application.Users;
using Quiz.Domain.Exams;
using Quiz.Domain.Exams.Sessions;
using Quiz.Domain.Users;

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

        /// <summary>
        /// 
        /// </summary>
        private void ExamSessionMaps() {
            CreateMap<ExamSessionResultsRequestDto, ExamSession>()
                ;

            CreateMap<AnswerDetailsDto, ExamSessionItem>()
                ;
        }
    }
}
