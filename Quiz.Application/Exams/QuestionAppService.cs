using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quiz.Application.Guids;
using Quiz.Domain;
using Quiz.Domain.Exams;
using Quiz.Domain.Extensions;

namespace Quiz.Application.Exams {
    public class QuestionAppService : QuizApplicationService<Question, QuestionDto, Guid>, IQuestionAppService {

        private readonly QuizDBContext _quizDBContext;
        private readonly string _dbProviderName;

        public QuestionAppService(
            ILogger<QuestionAppService> logger,
            IGuidGenerator guidGenerator,
            QuizDBContext dbContext,
            IMapper mapper) :
            base(logger, guidGenerator, dbContext, mapper) {
            _quizDBContext = dbContext;
            _dbProviderName = dbContext.Database.ProviderName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        public async Task<QuestionAndChoicesDto> FindBySearchKey(string searchKey) {
            if (string.IsNullOrWhiteSpace(searchKey)) return null;

            searchKey = searchKey.Trim();
            Guid.TryParse(searchKey, out Guid id);

            var entity = await _dbSet.Where(q => (q.Id == id) || (q.Code == searchKey))
                                    .Include(q => q.Choices)
                                    .FirstOrDefaultAsync();
            return (entity != null) ? _mapper.Map<QuestionAndChoicesDto>(entity) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public async Task<IList<QuestionAndChoicesDto>> GetQuestionsByIds(params Guid[] input) {
            var query = _dbSet //.Set<Question>()
                .Include(q => q.Choices)
                .Where(q => input.Contains(q.Id))
                //.OrderBy(sorting)
                ;
            return _mapper.Map<IList<QuestionAndChoicesDto>>(await query.ToListAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public async Task<IList<QuestionAndChoicesDto>> GetQuestionsByExam(ExamQuestionsRequestDto input) {

            // take the group of exams from which to retrieve the questions
            IList<Guid> examIds = input.IsRecursive
                ? await DoGetRecursiveExamIds(_quizDBContext.Exams.Where(ex => ex.Id == input.ExamId), 10)
                : new List<Guid>() { input.ExamId.Value };

            // if there are no exam ids to select, return an empty list
            if (!examIds.Any()) return new QuestionAndChoicesDto[0];

            // define sorting
            // FIXME: sorting func does not work with "input.Sorting" (as I'd later call OrderBy(string)
            Expression<Func<Question, object?>> sortingFunc = q =>
                input.IsRandom
                    ? Guid.NewGuid()
                    : !string.IsNullOrEmpty(input.Sorting)
                        ? input.Sorting : q.Position;

            // run the query over the questions to get all with the proper exam id
            List<QuestionAndChoicesDto> questionsDto = new List<QuestionAndChoicesDto>(input.MaxResultCount);

            // if user asks only for new questions, add only new questions
            if (input.OnlyNew) {
                questionsDto.AddRange(
                    await DoGetOnlyNewQuestions(
                            examIds,
                            sortingFunc,
                            input.IsRandom,
                            input.SkipCount,
                            input.MaxResultCount,
                            input.CandidateId)
                    );
            }

            // else if user asks only for previous errors/doubts add only them
            else if (input.OnlyErrorOrDoubt) {
                questionsDto.AddRange(
                    await DoGetOnlyOldErrorsOrDoubts(
                            examIds,
                            sortingFunc,
                            input.IsRandom,
                            input.SkipCount,
                            input.MaxResultCount,
                            input.CandidateId)
                    );
            }

            // else if sequential, get only sequental questions
            else {
                questionsDto.AddRange(
                    await DoGetAMix(
                            examIds,
                            sortingFunc,
                            input.IsRandom,
                            input.SkipCount,
                            input.MaxResultCount,
                            input.CandidateId)
                    );
            }

            // now sort again the entire set of entities and return
            return input.IsRandom
                ? questionsDto.AsQueryable().OrderBy(q => Guid.NewGuid()).ToList()
                : !string.IsNullOrEmpty(input.Sorting)
                    ? questionsDto.AsQueryable().OrderBy(input.Sorting).ToList()
                    : questionsDto.AsQueryable().OrderBy(q => q.Position).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<int> CreateQuestion(CreateQuestionDto input) {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<int> UpdateQuestion(UpdateQuestionDto input) {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="exams"></param>
        /// <param name="maxDeep"></param>
        /// <returns></returns>
        private async Task<IList<Guid>> DoGetRecursiveExamIds(IQueryable<Exam> exams, int maxDeep = 10) {
            var result = new List<Guid>(await exams.Select(ex => ex.Id).Distinct().ToListAsync());
            if (maxDeep > 0) {
                var qry = _quizDBContext.Exams.Where(ex => ex.AncestorId != null && exams.Select(exx => exx.Id).Contains(ex.AncestorId.Value));
                if (qry.Any()) {
                    result.AddRange(await DoGetRecursiveExamIds(qry, --maxDeep));
                }
            }
            return result.Distinct().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examIds"></param>
        /// <param name="sorting"></param>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private async Task<ICollection<QuestionAndChoicesDto>> DoGetOnlyNewQuestions(
            IList<Guid> examIds,
            Expression<Func<Question, object?>> sorting,
            bool isRandom,
            int skipCount,
            int maxResultCount,
            Guid? candidateId = null) {
            if (maxResultCount <= 0) throw new ArgumentOutOfRangeException(nameof(maxResultCount), $"{nameof(maxResultCount)} must be greater than 0");

            // get Questions Ids already used
            var qrySessionsIds = _quizDBContext.ExamSessions
                                        .WhereIf(candidateId.HasValue, session => session.CandidateId == candidateId.ToString())
                                        .Where(session => session.IsEnded)
                                        .Select(session => session.Id);
            var oldQuestionsIds = await _quizDBContext.ExamSessionItems
                        .Where(item => qrySessionsIds.Contains(item.SessionId) && item.IsAnswered)
                        .Select(item => item.QuestionId)
                        .Distinct()
                        .ToListAsync();
            //if (oldQuestionsIds.Count == 0) return new Question[] { };

            IList<Question>? questions = null;
            if (isRandom && _dbProviderName.Contains("sqlite", StringComparison.OrdinalIgnoreCase)) {
                // this is a SQLite database
                var queryStr = _dbSet
                    .Where(q => examIds.Contains(q.ExamId))
                    .Where(q => !oldQuestionsIds.Contains(q.Id))
                    .ToQueryString();
                queryStr = "SELECT * " +
                        queryStr.Substring(queryStr.IndexOf("FROM", StringComparison.OrdinalIgnoreCase)) +
                        $" ORDER BY RANDOM() LIMIT {maxResultCount} OFFSET {skipCount}";
                var ids = await _dbSet.FromSqlRaw(queryStr).Select(q => q.Id).ToListAsync();
                questions = (ids.Count <= 0)
                    ? new List<Question>(0)
                    : await _dbSet //.Set<Question>()
                                .Include(q => q.Choices)
                                .Where(q => ids.Contains(q.Id))
                                .ToListAsync();
            } else {
                // go forward with pure LINQ
                questions = await _dbSet //.Set<Question>()
                    .Include(q => q.Choices)
                    .Where(q => examIds.Contains(q.ExamId))
                    .Where(q => !oldQuestionsIds.Contains(q.Id))
                    .OrderBy(sorting)
                    .Skip(skipCount)
                    .Take(maxResultCount)
                    .ToListAsync();
            }

            return _mapper.Map<IList<QuestionAndChoicesDto>>(questions);
        }

        /// <summary>
        ///     get a set of questions for the new session with only questions in error in previous sessions 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private async Task<ICollection<QuestionAndChoicesDto>> DoGetOnlyOldErrorsOrDoubts(
            IList<Guid> examIds,
            Expression<Func<Question, object?>> sorting,
            bool isRandom,
            int skipCount,
            int maxResultCount,
            Guid? candidateId = null) {
            if (maxResultCount <= 0) throw new ArgumentOutOfRangeException(nameof(maxResultCount), $"{nameof(maxResultCount)} must be greater than 0");

            // if old errors or doubts exists get their ids
            var qry = _quizDBContext.CandidateNotes
                .Where(item => examIds.Contains(item.ExamId))
                .Where(item => !item.IsMarkedAsHidden && (item.NumOfWrongAnswers > 0 || item.IsMarkedAsDoubt))
                .WhereIf(candidateId.HasValue, item => item.CandidateId == candidateId.ToString());
            var errorsOrDoubtsIds = await
                qry
                .Select(item => new { item.QuestionId, item.IsMarkedAsDoubt })
                .Distinct()
                .ToListAsync();
            if (errorsOrDoubtsIds.Count == 0) return new QuestionAndChoicesDto[] { };

            // get all previous error related to the building exam session 
            IList<Question>? questions = null;
            if (isRandom && _dbProviderName.Contains("sqlite", StringComparison.OrdinalIgnoreCase)) {
                // this is a SQLite database
                var queryStr = _dbSet
                        .Where(q => errorsOrDoubtsIds.Select(x => x.QuestionId).Contains(q.Id))
                        .ToQueryString();
                queryStr = "SELECT * " +
                        queryStr.Substring(queryStr.IndexOf("FROM", StringComparison.OrdinalIgnoreCase)) +
                        $" ORDER BY RANDOM() LIMIT {maxResultCount} OFFSET {skipCount}";
                var ids = await _dbSet.FromSqlRaw(queryStr).Select(q => q.Id).ToListAsync();
                questions = (ids.Count <= 0)
                    ? new List<Question>(0)
                    : await _dbSet //.Set<Question>()
                                .Include(q => q.Choices)
                                .Where(q => ids.Contains(q.Id))
                                .ToListAsync();
            }
            else {
                // go forward with pure LINQ
                questions = await _dbSet //.Set<Question>()
                    .Include(q => q.Choices)
                    .Where(q => errorsOrDoubtsIds.Select(x => x.QuestionId).Contains(q.Id))
                    .OrderBy(sorting)
                    .Skip(skipCount)
                    .Take(maxResultCount)
                    .ToListAsync();
            }

            var dtos = _mapper.Map<IList<QuestionAndChoicesDto>>(questions);
            foreach (var sitem in errorsOrDoubtsIds.Where(si => si.IsMarkedAsDoubt).ToArray()) {
                var dto = dtos.FirstOrDefault(q => q.Id == sitem.QuestionId);
                if (dto != null) dto.IsMarkedAsDoubt = sitem.IsMarkedAsDoubt;
            }

            return dtos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        ///     get a set of questions for the new session with only questions in error in previous sessions 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private async Task<IEnumerable<QuestionAndChoicesDto>> DoGetAMix(
            IList<Guid> examIds, Expression<Func<Question, object>>
            sortingFunc,
            bool isRandom,
            int skipCount,
            int maxResultCount,
            Guid? candidateId = null) {

            List<QuestionAndChoicesDto>? questionsDto = new List<QuestionAndChoicesDto>(maxResultCount);

            // if user asks for random questions get some previous error
            if (isRandom) {
                // add at least 20% of previous error or questions marked as doubt
                int percentage = 20;
                questionsDto.AddRange(
                    await DoGetOnlyOldErrorsOrDoubts(
                            examIds,
                            sortingFunc,
                            isRandom,
                            skipCount,
                            (int)Math.Ceiling(maxResultCount * percentage / 100.0),
                            candidateId)
                    );

            }

            // add the questions for the exam
            if (questionsDto.Count < maxResultCount) {
                var alreadySelected = questionsDto.Select(q => q.Id).ToList();

                IList<Question>? newQuestions = null;
                if (isRandom && _dbProviderName.Contains("sqlite", StringComparison.OrdinalIgnoreCase)) {
                    // this is a SQLite database
                    var queryStr = _dbSet
                                .Where(q => examIds.Contains(q.ExamId))
                                .Where(q => !alreadySelected.Contains(q.Id))
                                .ToQueryString();
                    queryStr = "SELECT * " +
                            queryStr.Substring(queryStr.IndexOf("FROM", StringComparison.OrdinalIgnoreCase)) +
                            $" ORDER BY RANDOM() LIMIT {maxResultCount - alreadySelected.Count} OFFSET {skipCount}";
                    var qryIds = _dbSet
                                .FromSqlRaw(queryStr)
                                .Select(q => q.Id);
                    newQuestions = await _dbSet //.Set<Question>()
                                .Include(q => q.Choices)
                                .Where(q => qryIds.Contains(q.Id))
                                .ToListAsync();
                }
                else {
                    // go forward with pure LINQ
                    newQuestions = await _dbSet //.Set<Question>()
                                .Include(q => q.Choices)
                                .Where(q => examIds.Contains(q.ExamId))
                                .Where(q => !alreadySelected.Contains(q.Id))
                                .OrderBy(sortingFunc)
                                .Skip(skipCount)
                                .Take(maxResultCount - alreadySelected.Count)
                                .ToListAsync();
                }

                var newQuestionsDto = _mapper.Map<IList<QuestionAndChoicesDto>>(newQuestions);

                // if old doubt exists get their question id
                var qry = _quizDBContext.CandidateNotes
                    .Where(cnote => newQuestionsDto.Select(q => q.Id).Contains(cnote.QuestionId))
                    .Where(item => !item.IsMarkedAsHidden && item.IsMarkedAsDoubt)
                    .WhereIf(candidateId.HasValue, item => item.CandidateId == candidateId.ToString());
                var doubts = await qry
                                .Select(item => new { item.QuestionId, item.IsMarkedAsDoubt })
                                .Distinct()
                                .ToListAsync();
                foreach (var cnote in doubts) {
                    var dto = newQuestionsDto.FirstOrDefault(q => q.Id == cnote.QuestionId);
                    if (dto != null) dto.IsMarkedAsDoubt = cnote.IsMarkedAsDoubt;
                }

                questionsDto.AddRange(newQuestionsDto);
            }

            return questionsDto;
        }

    }
}
