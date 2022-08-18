using AutoMapper;
using Quiz.Application.Guids;
using Quiz.Tests.Base;

namespace Quiz.Tests.Application {
    public abstract class QuizApplicationTestBase : QuizTestBase {

        protected static readonly IGuidGenerator _guidGenerator = new SequentialGuidGenerator();
        protected readonly IMapper _mapper;

        public QuizApplicationTestBase() {
            //auto mapper configuration
            _mapper = new MapperConfiguration(cfg => {
                cfg.AddProfile(new AutomapperProfile());
            }).CreateMapper();

        }
    }
}