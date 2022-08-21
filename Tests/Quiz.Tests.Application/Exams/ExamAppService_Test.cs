using Quiz.Domain.Exams;

namespace Quiz.Application.Exams.Tests {

    public class ExamAppService_Test : QuizApplicationTestBase {

        private readonly IExamAppService _examService;

        public ExamAppService_Test() {
            _examService = new ExamAppService(
                Mock.Of<ILogger<ExamAppService>>(),
                _guidGenerator,
                QuizDbContext,
                QuizIdentityDbContext,
                _mapper);
        }

        [Fact]
        public async Task FindById() {
            // prepare
            Exam entity = await CreateAndInsertRootExam();

            // act
            var output = await _examService.FindById(entity.Id);

            // assert
            output.ShouldNotBeNull();
            output.Id.ShouldBe(entity.Id);
            output.AncestorId.ShouldBe(entity.AncestorId);
            output.Name.ShouldBe(entity.Name);
            output.Code.ShouldBe(entity.Code);
            output.Duration.ShouldBe(entity.Duration);
            output.FullMarks.ShouldBe(entity.FullMarks);

            // act
            output = await _examService.FindById(Guid.Empty);
            output.ShouldBeNull();
        }

        [Fact]
        public async Task GetAllRootExams() {
            // prepare
            var nRoots = QuizDbContext.Exams.Count(ex => !ex.IsDeleted.GetValueOrDefault() && (ex.Ancestor == null));
            // act
            var dtos = await _examService.GetAllRootExams();
            // assert
            dtos.ShouldNotBeNull();
            dtos.Count.ShouldBe(nRoots);
            dtos.ShouldAllBe(ex => ex.Ancestor == null);

            // prepare
            var root1 = await CreateAndInsertRootExam();
            await CreateAndInsertChildExam(root1);
            await CreateAndInsertChildExam(root1);
            await CreateAndInsertChildExam(await CreateAndInsertChildExam(await CreateAndInsertChildExam(root1)));
            var root2 = await CreateAndInsertRootExam();
            await CreateAndInsertChildExam(root2);
            await CreateAndInsertChildExam(root2);

            // act
            dtos = await _examService.GetAllRootExams();

            // assert
            dtos.ShouldNotBeNull();
            dtos.Count.ShouldBe(nRoots + 2);
            dtos.ShouldAllBe(ex => ex.Ancestor == null);
        }

        [Fact()]
        public async Task GetRecursiveExamIds() {
            // prepare
            var root1 = await CreateAndInsertRootExam();
            await CreateAndInsertChildExam(root1);
            await CreateAndInsertChildExam(root1);
            await CreateAndInsertChildExam(await CreateAndInsertChildExam(await CreateAndInsertChildExam(root1)));
            var root2 = await CreateAndInsertRootExam();
            await CreateAndInsertChildExam(root2);
            await CreateAndInsertChildExam(root2);

            // act
            var dto = await _examService.GetRecursiveExamIds(new RecursiveExamsRequestDto(root2.Id, maxDeep: 0));
            // assert
            dto.ShouldNotBeNull();
            dto.Count.ShouldBe(1);
            dto[0].ShouldBe(root2.Id);

            // act
            dto = await _examService.GetRecursiveExamIds(new RecursiveExamsRequestDto(root1.Id, maxDeep: 1));
            // assert
            dto.ShouldNotBeNull();
            dto.Count.ShouldBe(4);
            dto[0].ShouldBe(root1.Id);

            // act
            dto = await _examService.GetRecursiveExamIds(new RecursiveExamsRequestDto(root1.Id, maxDeep: 10));
            // assert
            dto.ShouldNotBeNull();
            dto.Count.ShouldBe(6);
            dto[0].ShouldBe(root1.Id);
        }

        [Fact()]
        public async Task UpdateExam() {
            // prepare
            var root1 = await CreateAndInsertRootExam();
            await CreateAndInsertChildExam(root1);
            await CreateAndInsertChildExam(root1);
            await CreateAndInsertChildExam(await CreateAndInsertChildExam(await CreateAndInsertChildExam(root1)));
            var root2 = await CreateAndInsertRootExam();
            await CreateAndInsertChildExam(root2);
            await CreateAndInsertChildExam(root2);

            var input = new UpdateExamDto {
                Id = root1.Id,
                AncestorId = root1.AncestorId,
                Name = "Modified name",
                Code = root1.Code,
                Duration = 1234,
                FullMarks = root1.FullMarks,
            };

            // act
            var output = await _examService.UpdateExam(input);

            // assert
            output.ShouldBe(1);

            var entity = QuizDbContext.Exams.FirstOrDefault(ex => ex.Id == input.Id);
            entity.ShouldNotBeNull();
            entity.AncestorId.ShouldBe(input.AncestorId);
            entity.Code.ShouldBe(input.Code);
            entity.Duration.ShouldBe(input.Duration);
            entity.FullMarks.ShouldBe(input.FullMarks);
            entity.Name.ShouldBe(input.Name);

        }
    }
}