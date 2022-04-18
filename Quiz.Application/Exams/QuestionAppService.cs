using System.Linq.Dynamic.Core;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Quiz.Domain;
using Quiz.Domain.Exams;


namespace Quiz.Application.Exams {
    public class QuestionAppService : QuizApplicationService<Question, QuestionDto, Guid>, IQuestionAppService {

        public QuestionAppService(QuizDBContext dbContext, IMapper mapper) : base(dbContext, mapper) {
        }

        private async Task<IList<Guid>> GetRecursiveExamIds(IQueryable<Exam> exams, int maxDeep = 100) {
            var result = new List<Guid>(await exams.Select(ex => ex.Id).Distinct().ToListAsync());
            if (maxDeep > 0) {
                var qry = _dbContext.Exams.Where(ex => ex.AncestorId != null && exams.Select(exx => exx.Id).Contains(ex.AncestorId.Value));
                if (qry.Any()) {
                    result.AddRange(await GetRecursiveExamIds(qry, --maxDeep));
                }
            }
            return result.Distinct().ToList();
        }

        public async Task<ICollection<QuestionAndChoicesDto>> GetRecursiveQuestionsByExam(PrepareExamSessionRequestDto input) {
            IList<Guid> examIds = input.IsRecursive
                ? await GetRecursiveExamIds(_dbContext.Exams.Where(ex => ex.Id == input.ExamId), 10)
                : new List<Guid>() { input.ExamId };

            // if there are no exam ids to select, return an empty list
            if (!examIds.Any()) return new QuestionAndChoicesDto[0];

            // run the query over the questions to get all with the proper exam id
            var query = _dbSet //.Set<Question>()
                .Include(q => q.Choices)
                .Where(q => examIds.Contains(q.ExamId) /*&& q.ImageUri != null*/);
            if (input.IsRandom) {
                query = query.OrderBy(q => Guid.NewGuid());
            } else if (!string.IsNullOrEmpty(input.Sorting)) {
                query = query.OrderBy(input.Sorting);
            } else {
                query = query.OrderBy(q => q.Position);
            }
            //query = (input.IsRandom)
            //    ? query.OrderBy(q => Guid.NewGuid())
            //    : !string.IsNullOrEmpty(input.Sorting)
            //        ? query.OrderBy(input.Sorting)
            //        : query.OrderBy(q => q.Code)/*.ThenBy(q => q.Code)*/;
            query = query/*.AsNoTracking()*/.Skip(input.SkipCount).Take(input.MaxResultCount);

            var entities = await query.ToListAsync();

            //var errori = entities.Where(e => (e.Choices?.Count ?? 0) == 0).ToList();

            return _mapper.Map<QuestionAndChoicesDto[]>(entities);
        }


        public async Task<PrepareExamSessionResponseDto> PrepareExamSession(PrepareExamSessionRequestDto input) {
            // get the exam title/name
            var exam = await _dbContext.Exams.FirstOrDefaultAsync(e => e.Id == input.ExamId);
            if (exam == null) throw new Exception($"Exam not found (id: ${input.ExamId})");
            // get the questions
            var questions = await GetRecursiveQuestionsByExam(input);

            return new PrepareExamSessionResponseDto {
                ExamId = input.ExamId,
                Name = exam.Name,
                Duration = exam.Duration,
                Questions = questions,
            };
        }

        public Task<int> UpdateQuestion(Question entity) {
            throw new NotImplementedException();
        }

    }
}
