using System.Linq.Dynamic.Core;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quiz.Application.Exams.Sessions;
using Quiz.Application.Guids;
using Quiz.Application.Users;
using Quiz.Domain;
using Quiz.Domain.Exams;


namespace Quiz.Application.Exams {
    public class QuestionAppService : QuizApplicationService<Question, QuestionDto, Guid>, IQuestionAppService {

        public QuestionAppService(
            ILogger<QuestionAppService> logger,
            IGuidGenerator guidGenerator,
            QuizDBContext dbContext,
            IMapper mapper) :
            base(logger, guidGenerator, dbContext, mapper) {
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public async Task<ICollection<QuestionAndChoicesDto>> GetRecursiveQuestionsByExam(
            PrepareExamSessionRequestDto input, BasicCandidateDto candidate = null) {

            // take the group of exams from which to retrieve the questions
            IList<Guid> examIds = input.IsRecursive
                ? await GetRecursiveExamIds(_dbContext.Exams.Where(ex => ex.Id == input.ExamId), 10)
                : new List<Guid>() { input.ExamId };

            // if there are no exam ids to select, return an empty list
            if (!examIds.Any()) return new QuestionAndChoicesDto[0];

            // run the query over the questions to get all with the proper exam id

            List<Question> entities = new List<Question>(input.MaxResultCount);

            if (input.IsRandom && (candidate != null)) {
                // get at least the the 20% of questions with wrong answers in the past                
                //entities.AddRange(await DoGetErrorFromPreviousSessionsV1(input, candidate.Id, examIds, 20));
                entities.AddRange(await DoGetErrorFromPreviousSessionsV2(input, candidate.Id, examIds, 20));
            }

            // if (the wrong questions are not enought, get new one!)
            if (entities.Count < input.MaxResultCount) {
                var alreadySelected = entities.Select(q => q.Id).ToList();
                var query = _dbSet //.Set<Question>()
                    .Include(q => q.Choices)
                    .Where(q => examIds.Contains(q.ExamId))
                    .Where(q => !alreadySelected.Contains(q.Id))
                    ;
                query = input.IsRandom
                    ? query.OrderBy(q => Guid.NewGuid())
                    : !string.IsNullOrEmpty(input.Sorting)
                        ? query.OrderBy(input.Sorting)
                        : query.OrderBy(q => q.Position);
                query = query/*.AsNoTracking()*/.Skip(input.SkipCount).Take(input.MaxResultCount - alreadySelected.Count);
                entities.AddRange(await query.ToListAsync());
            }

            // now shuffle again the entities, if needed
            if (input.IsRandom) entities = entities.OrderBy(a => Guid.NewGuid()).ToList();

            return _mapper.Map<QuestionAndChoicesDto[]>(entities);
        }

        private async Task<ICollection<Question>> DoGetErrorFromPreviousSessionsV1(
            PrepareExamSessionRequestDto input, Guid candidateId, IList<Guid> examIds, int percentage) {

            if (percentage <= 0 || percentage > 100) throw new ArgumentOutOfRangeException(nameof(percentage), $"{nameof(percentage)} must be between 1 and 100");

            // get all previous error related to the building exam session 
            var errorsOrDoubtsId =
                await _dbContext.ExamSessionItems
                    .Where(item =>
                        _dbContext.ExamSessions
                        .Where(es => es.CandidateId == candidateId)
                        .Where(es => examIds.Contains(es.ExamId))
                        .Select(es => es.Id)
                        .Distinct()
                        .Contains(item.SessionId))
                    .Where(item => !item.IsCorrect || item.IsMarkedAsDoubt)
                    .Select(item => item.QuestionId)
                    .ToListAsync();
            if (errorsOrDoubtsId.Count == 0) return new Question[] { };

            IQueryable<Question> query = _dbSet //.Set<Question>()
                .Include(q => q.Choices)
                .Where(q => errorsOrDoubtsId.Contains(q.Id))
                .OrderBy(q => Guid.NewGuid());
            query = input.OnlyErrorOrDoubt  // if true try to get all wrong anwers
                ? query/*.Skip(input.SkipCount)*/.Take(input.MaxResultCount)
                : query/*.Skip(input.SkipCount)*/.Take((int)Math.Ceiling(input.MaxResultCount * percentage / 100.0));

            return await query.ToListAsync();
        }

        // get a set of questions for the new session with only questions in error in previous sessions
        private async Task<ICollection<Question>> DoGetErrorFromPreviousSessionsV2(
            PrepareExamSessionRequestDto input, Guid candidateId, IList<Guid> examIds, int percentage) {

            if (percentage <= 0 || percentage > 100) throw new ArgumentOutOfRangeException(nameof(percentage), $"{nameof(percentage)} must be between 1 and 100");

            // get all previous error related to the building exam session 
            var errorsOrDoubtsId =
                await _dbContext.CandidateNotes
                    .Where(item => item.CandidateId == candidateId)
                    .Where(item => examIds.Contains(item.ExamId))
                    .Where(item => !item.IsMarkedAsHidden && (item.NumOfWrongAnswers > 0 || item.IsMarkedAsDoubt))
                    .Select(item => item.QuestionId)
                    .ToListAsync();
            if (errorsOrDoubtsId.Count == 0) return new Question[] { };

            // get a set of questions for the new session with only questions in error in previous sessions
            IQueryable<Question> query = _dbSet //.Set<Question>()
                .Include(q => q.Choices)
                .Where(q => errorsOrDoubtsId.Contains(q.Id))
                .OrderBy(q => Guid.NewGuid());
            query = input.OnlyErrorOrDoubt  // if true try to get all wrong anwers
                ? query/*.Skip(input.SkipCount)*/.Take(input.MaxResultCount)
                : query/*.Skip(input.SkipCount)*/.Take((int)Math.Ceiling(input.MaxResultCount * percentage / 100.0));

            return await query.ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<int> UpdateQuestion(Question entity) {
            throw new NotImplementedException();
        }

    }
}
