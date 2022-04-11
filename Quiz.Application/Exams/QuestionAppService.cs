using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Quiz.Domain;
using Quiz.Domain.Exams;

namespace Quiz.Application.Exams {
    public class QuestionAppService : QuizApplicationService<Question, QuestionDto, Guid>, IQuestionAppService {

        public QuestionAppService(QuizDBContext dbContext, IMapper mapper) : base(dbContext, mapper) {
        }

        public async Task<ICollection<QuestionAndChoicesDto>> GetRecursiveQuestionsByExam(PrepareExamSessionRequestDto input) {
            // get the exam ids
            IList<Guid> examIds = null;
            if (!input.IsRecursive) {
                examIds = new List<Guid>() { input.ExamId };
            } else {
                var qryExamIds1stLevel = _dbContext.Exams
                    .Where(e => e.Id == input.ExamId || e.AncestorId == input.ExamId)
                    .Select(e => e.Id);
                examIds = await _dbContext.Exams
                    .Where(e => qryExamIds1stLevel.Contains(e.Id))
                    //.Select(e => new { e.Id, e.Code })
                    .Select(e => e.Id)
                    .ToListAsync();
            }

            // if there are no exam ids to select, return an empty list
            if (!examIds.Any()) return new QuestionAndChoicesDto[0];

            // run the query over the questions to get all with the proper exam id
            var query = _dbSet //.Set<Question>()
                .Include(q => q.Choices)
                .Where(q => examIds.Contains(q.ExamId) /*&& q.ImageUri != null*/);
            query = (input.IsRandom)
                ? query.OrderBy(q => Guid.NewGuid())
                : !string.IsNullOrEmpty(input.Sorting)
                    ? query.OrderBy(input.Sorting)
                    : query.OrderBy(q => q.ExamId).ThenBy(q => q.Code);
            query = query.AsNoTracking().Skip(input.SkipCount).Take(input.MaxResultCount);

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
