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

        public QuestionAppService(
            ILogger<QuestionAppService> logger,
            IGuidGenerator guidGenerator,
            QuizDBContext dbContext,
            QuizIdentityDBContext dbIdentityContext,
            IMapper mapper) :
            base(logger, guidGenerator, dbContext, dbIdentityContext, mapper) {
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
                ? await DoGetRecursiveExamIds(_dbContext.Exams.Where(ex => ex.Id == input.ExamId), 10)
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
                            input.SkipCount,
                            input.MaxResultCount,
                            input.CandidateId)
                    );
            }

            // else if sequential, get only sequental questions
            else {
                // if user asks for random questions get some previous error
                if (input.IsRandom) {
                    // add at least 20% of previous error or questions marked as doubt
                    int percentage = 20;
                    questionsDto.AddRange(
                        await DoGetOnlyOldErrorsOrDoubts(
                                examIds,
                                sortingFunc,
                                input.SkipCount,
                                (int)Math.Ceiling(input.MaxResultCount * percentage / 100.0),
                                input.CandidateId)
                        );
                }

                // add the questions for the exam
                if (questionsDto.Count < input.MaxResultCount) {
                    var alreadySelected = questionsDto.Select(q => q.Id).ToList();
                    var newQuestionsDto = _mapper.Map<IList<QuestionAndChoicesDto>>(
                                                            await _dbSet //.Set<Question>()
                                                            .Include(q => q.Choices)
                                                            .Where(q => examIds.Contains(q.ExamId))
                                                            .Where(q => !alreadySelected.Contains(q.Id))
                                                            .OrderBy(sortingFunc)
                                                            .Skip(input.SkipCount)
                                                            .Take(input.MaxResultCount - alreadySelected.Count)
                                                            .ToListAsync());

                    // if old doubt exists get their question id
                    var qry = _dbContext.CandidateNotes
                        .Where(cnote => newQuestionsDto.Select(q => q.Id).Contains(cnote.QuestionId))
                        .Where(item => !item.IsMarkedAsHidden && item.IsMarkedAsDoubt)
                        .WhereIf(input.CandidateId.HasValue, item => item.CandidateId == input.CandidateId.ToString());
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
                var qry = _dbContext.Exams.Where(ex => ex.AncestorId != null && exams.Select(exx => exx.Id).Contains(ex.AncestorId.Value));
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
            int skipCount,
            int maxResultCount,
            Guid? candidateId = null) {
            if (maxResultCount <= 0) throw new ArgumentOutOfRangeException(nameof(maxResultCount), $"{nameof(maxResultCount)} must be greater than 0");

            // get Questions Ids already used
            var qrySessionsIds = _dbContext.ExamSessions
                                        .WhereIf(candidateId.HasValue, session => session.CandidateId == candidateId.ToString())
                                        .Where(session => session.IsEnded)
                                        .Select(session => session.Id);
            var oldQuestionsIds = await _dbContext.ExamSessionItems
                        .Where(item => qrySessionsIds.Contains(item.SessionId) && item.IsAnswered)
                        .Select(item => item.QuestionId)
                        .Distinct()
                        .ToListAsync();
            //if (oldQuestionsIds.Count == 0) return new Question[] { };

            var query = _dbSet //.Set<Question>()
                .Include(q => q.Choices)
                .Where(q => examIds.Contains(q.ExamId))
                .Where(q => !oldQuestionsIds.Contains(q.Id))
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount);
            return _mapper.Map<IList<QuestionAndChoicesDto>>(await query.ToListAsync());
        }

        /// <summary>
        ///     get a set of questions for the new session with only questions in error in previous sessions 
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private async Task<ICollection<QuestionAndChoicesDto>> DoGetOnlyOldErrorsOrDoubts(
            IList<Guid> examIds,
            Expression<Func<Question, object?>> sorting,
            int skipCount,
            int maxResultCount,
            Guid? candidateId = null) {
            if (maxResultCount <= 0) throw new ArgumentOutOfRangeException(nameof(maxResultCount), $"{nameof(maxResultCount)} must be greater than 0");

            // if old errors or doubts exists get their ids
            var qry = _dbContext.CandidateNotes
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
            var questions = await _dbSet //.Set<Question>()
                .Include(q => q.Choices)
                .Where(q => errorsOrDoubtsIds.Select(x => x.QuestionId) .Contains(q.Id))
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();

            var dtos = _mapper.Map<IList<QuestionAndChoicesDto>>(questions);
            foreach (var sitem in errorsOrDoubtsIds.Where(si => si.IsMarkedAsDoubt).ToArray()) {
                var dto = dtos.FirstOrDefault(q => q.Id == sitem.QuestionId);
                if (dto != null) dto.IsMarkedAsDoubt = sitem.IsMarkedAsDoubt;
            }

            return dtos;
        }

    }
}
