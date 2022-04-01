using AutoMapper;
using PatenteN.Quiz.Domain.Exams;
using PatenteN.Quiz.Domain.Users;

namespace PatenteN.Quiz.Application {

    internal class AutomapperProfile : Profile {

        public AutomapperProfile() {
            CandidateMaps();
            ExamMaps();
            ExamQuestions();
        }

        private void CandidateMaps() {
            CreateMap<Candidate, CandidateDto>()
                ;

            CreateMap<CandidateDto, Candidate>()
                //.ForMember(dst => dst.CreatedOn, src => src.Ignore())
                ;
        }

        private void ExamMaps() {
            CreateMap<Exam, ExamDto>()
                ;

            CreateMap<ExamDto, Exam>()
                //.ForMember(dst => dst.CreatedOn, src => src.Ignore())
                ;
        }

        private void ExamQuestions() {
            CreateMap<Question, QuestionDto>()
                ;

            CreateMap<QuestionDto, Question>()
                //.ForMember(dst => dst.CreatedOn, src => src.Ignore())
                ;
        }
    }
}
