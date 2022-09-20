using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quiz.Application.Exams;
using Quiz.Application.Guids;
using Quiz.Application.Users;
using Quiz.Domain;
using Quiz.Domain.Exams;
using Quiz.Domain.Exams.Sessions;
using Quiz.Domain.Extensions;

namespace Quiz.Application.Sessions {

    // TODO [Microsoft.AspNetCore.Authorization.Authorize(Roles = $"{QuizConstants.Roles.Candidate}, {QuizConstants.Roles.Manager}, {QuizConstants.Roles.Admin}")]
    public class ExamSessionAppService : QuizApplicationService<ExamSession, ExamSessionDto, Guid>, IExamSessionAppService {
        private readonly QuizDBContext _quizDBContext;
        private readonly ICandidateAppService _candidateAppService;
        private readonly IExamAppService _examAppService;
        private readonly IQuestionAppService _questionAppService;

        public ExamSessionAppService(
            ILogger<ExamSessionAppService> logger,
            IGuidGenerator guidGenerator,
            QuizDBContext dbContext,
            IMapper mapper,
            ICandidateAppService candidateAppService,
            IExamAppService examAppService,
            IQuestionAppService questionAppService) :
            base(logger, guidGenerator, dbContext, mapper) {
            _quizDBContext = dbContext;
            _questionAppService = questionAppService;
            _candidateAppService = candidateAppService;
            _examAppService = examAppService;
        }

        /// <summary>
        ///		Prepare a new exam session by getting the requested exam's info and a subset of questions for the exam
        /// </summary>
        /// <exception cref="Exception">In case of error</exception>
        public async Task<PrepareExamSessionResponseDto> PrepareExamSession(PrepareExamSessionRequestDto input) {
            try {
                // get the exam title/name
                Exam exam = null;
                if (input.ExamId != null) {
                    exam = await _quizDBContext.Exams.FirstOrDefaultAsync(
                                        e => (e.Id == input.ExamId)
                                        //|| e.Id.ToString().Equals(input.ExamId.Value.ToString())
                                        );
                }
                if (exam == null) throw new Exception($"Exam not found (id: ${input.ExamId})");
                var output = _mapper.Map<PrepareExamSessionResponseDto>(exam);

                // get the questions and return
                output.Questions = await _questionAppService.GetQuestionsByExam(input);
                return output;
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        ///		Prepare a new exam session by getting the requested exam's info and a subset of questions from an old exam session
        /// </summary>
        /// <exception cref="Exception">In case of error</exception>
        public async Task<PrepareExamSessionResponseDto> PrepareExamSession(Guid oldSessionId) {
            // FIXME: only sessions owing to the current candidate should be accessible 
            try {
                // get the exam title/name
                var oldSession = await _quizDBContext.ExamSessions.Include(es => es.Exam).FirstOrDefaultAsync(es => es.Id == oldSessionId);
                if (oldSession == null) throw new Exception($"Exam session not found (id: ${oldSessionId})");
                var output = _mapper.Map<PrepareExamSessionResponseDto>(oldSession);

                // get the questions and return
                var guids = oldSession.QSequence.Split(',').Select(guid => Guid.Parse(guid)).ToArray();
                output.Questions = await _questionAppService.GetQuestionsByIds(guids);
                return output;
            }
            catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IList<ExamSessionDto>> GetUserSessions(UserSessionsRequestDto input) {
            // if examId is passed, get it, else get all root exams
            var rootExams = input.ExamId.HasValue
                ? await _examAppService.GetRecursiveExamIds(new RecursiveExamsRequestDto(input.ExamId.Value, input.MaxDeep))
                : (await _examAppService.GetAllRootExams()).Select(ex => ex.Id).ToList();

            // if not root exams return
            if (rootExams.Count <= 0) return new List<ExamSessionDto>();

            var cadidateId = input.CandidateId?.ToString();
            var query = _quizDBContext.ExamSessions
                            .WhereIf(!string.IsNullOrEmpty(cadidateId), es => cadidateId == es.CandidateId)
                            .Where(es => rootExams.Contains(es.ExamId))
                            .OrderByDescending(es => es.ModifiedOn)
                            .ThenByDescending(es => es.CreatedBy)
                            .ProjectTo<ExamSessionDto>(this._mapper.ConfigurationProvider);
            return await query.ToListAsync();
        }

        public Task<IList<ExamSessionWithAnswersDto>> GetUserSessionsWithAnswers(UserSessionsRequestDto input) {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<IList<SessionsStatisticsDto>> GetUserStats(UserSessionsRequestDto input) {
            // if examId is passed, get it, else get all root exams
            var rootExams = input.ExamId.HasValue
                ? (new ExamDto[] { await _examAppService.FindById(input.ExamId.Value) })
                : await _examAppService.GetAllRootExams();

            // if not root exams return
            if (rootExams.Count <= 0) return new List<SessionsStatisticsDto>();

            var candidateName = input.CandidateId.HasValue
                ? await _candidateAppService.GetCandidateName(input.CandidateId)
                : null;

            // FIXME too much results (mainly if CandidateId is null)
            var candidateSessions = await _quizDBContext.ExamSessions
                                                .WhereIf(input.CandidateId.HasValue, cd => cd.CandidateId == input.CandidateId.ToString())
                                                .Where(cd => cd.IsEnded)
                                                .ToListAsync();

            var output = new List<SessionsStatisticsDto>(rootExams.Count);
            foreach (var exam in rootExams) {
                var allExamIds = await _examAppService.GetRecursiveExamIds(new RecursiveExamsRequestDto(exam.Id, maxDeep: input.MaxDeep));
                var stat = new SessionsStatisticsDto {
                    CandidateId = input.CandidateId,
                    CandidateName = candidateName,
                    ExamId = exam.Id,
                    ExamName = exam.Name,
                    NumOfAvailableQuestions = await _quizDBContext.Questions.CountAsync(q => allExamIds.Contains(q.ExamId)),
                    NumOfCarriedOutSessions = candidateSessions.Count(cs => allExamIds.Contains(cs.ExamId)),
                    NumOfWrongAnswers = await _quizDBContext.CandidateNotes.CountAsync(cn => allExamIds.Contains(cn.ExamId) &&  (cn.CandidateId == input.CandidateId.ToString()) && (cn.NumOfWrongAnswers > 0)),
                    NumOfDoubtAnswers = await _quizDBContext.CandidateNotes.CountAsync(cn => allExamIds.Contains(cn.ExamId) &&  (cn.CandidateId == input.CandidateId.ToString()) && cn.IsMarkedAsDoubt),
                };

                //var v1 = await _quizDBContext.ExamSessionItems
                //        .Where(item => candidateSessions.Select(cs => cs.Id).Contains(item.SessionId) && allExamIds.Contains(item.ExamId) && item.IsAnswered)
                //        .Select(item => item.QuestionId)
                //        .Distinct()
                //        .CountAsync();

                // get all session of this root exam
                var sessionIds = _quizDBContext.ExamSessions.Where(es => allExamIds.Contains(es.ExamId)).Select(es => es.Id);
                // now calc the question never answered
                stat.NumOfNeverAnswered =
                    stat.NumOfAvailableQuestions -
                    await _quizDBContext.ExamSessionItems
                            .Where(item => candidateSessions.Select(cs => cs.Id).Contains(item.SessionId) && sessionIds.Contains(item.SessionId) && item.IsAnswered)
                            .Select(item => item.QuestionId)
                            .Distinct()
                            .CountAsync();

                output.Add(stat);
            }

            return output;
        }

        /// <summary>
        ///     Save a user Session
        /// </summary>
        public async Task<Guid> SaveUserSession(ExamSessionRequestDto input) {
            // validate input params
            if (!await _candidateAppService.Any(input.CandidateId ?? Guid.Empty))
                throw new ArgumentException("Cannot find the indicated candidate", nameof(input.CandidateId));
            if (!await _quizDBContext.Exams.AnyAsync(ex => ex.Id == input.ExamId))
                throw new ArgumentException("Cannot find the indicated exam", nameof(input.ExamId));

            // create the exam session
            var session = new ExamSession(GuidGenerator.Create(), input.CandidateId.Value, input.ExamId);
            _mapper.Map(input, session);
            session.ExecutedOn = DateTimeOffset.Now;
            session.NumOfQuestions = input.Answers.Count;
            session.NumOfCorrectAnswers = input.Answers.Count(ans => ans.IsCorrect);
            session.NumOfWrongAnswers = input.Answers.Count(ans => !ans.IsCorrect && ans.UserChoiceId.HasValue);
            session.QSequence = String.Join(',', input.Answers.Select(ans => ans.QuestionId.Value.ToString("D")).ToArray());
            session.ExamName = await _quizDBContext.Exams.Where(ex => ex.Id == input.ExamId).Select(ex => ex.Name).SingleAsync();

            _quizDBContext.ExamSessions.Add(session);

            // add the user's answers
            foreach (var dto in input.Answers) {
                var ans = new ExamSessionItem(GuidGenerator.Create(), session.Id);
                _mapper.Map(dto, ans);
                _quizDBContext.ExamSessionItems.Add(ans);
            }

            // add the user's errors, doubts, etc
            var candidateNotes = await _quizDBContext.CandidateNotes.Where(cn => cn.CandidateId == input.CandidateId.ToString()).ToListAsync();
            foreach (var answDto in input.Answers) {
                // skip unanswered, if requested
                if (input.SkipUnanswered && !answDto.IsAnswered) continue;

                var cn = candidateNotes.FirstOrDefault(cn => cn.QuestionId == answDto.QuestionId)
                            ?? new CandidateNote(input.CandidateId.Value, answDto.QuestionId.Value, answDto.ExamId.Value);
                cn.ModifiedOn = DateTimeOffset.Now;
                cn.IsMarkedAsDoubt = answDto.IsMarkedAsDoubt;
                cn.IsMarkedAsHidden = answDto.IsMarkedAsHidden;
                if (answDto.IsCorrect) cn.SubErrorCount(); else cn.AddErrorCount();

                // is the candidate note a new one?
                var isNewCn = (_quizDBContext.Entry(cn).State == EntityState.Detached);

                // check if it is required to be removed
                if (!isNewCn && (cn.NumOfWrongAnswers <= 0) && !cn.IsMarkedAsDoubt && !cn.IsMarkedAsHidden) {
                    // delete it
                    _quizDBContext.CandidateNotes.Remove(cn);
                } else if (isNewCn && (cn.NumOfWrongAnswers > 0 || cn.IsMarkedAsDoubt || cn.IsMarkedAsHidden)) {
                    // add it
                    _quizDBContext.CandidateNotes.Add(cn);
                    candidateNotes.Add(cn);
                }
            }

            //// delete all unseless notes
            //_quizDBContext.CandidateNotes.RemoveRange(_quizDBContext.CandidateNotes.Where(cn => (cn.NumOrWrongAnswers <= 0) && !cn.IsMarkedAsDoubt && !cn.IsMarkedAsHidden);

            // commit all
            int nSaved = await _quizDBContext.SaveChangesAsync();
            if (nSaved <= 0) throw new Exception();
            return session.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> DeleteUserSessions(UserSessionsRequestDto input) {
            IList<Guid> allExamIds = (input.ExamId != null)
                ? await _examAppService.GetRecursiveExamIds(
                            new RecursiveExamsRequestDto(input.ExamId.Value,input.MaxDeep))
                : null;
            _quizDBContext.ExamSessions.RemoveRange(
               await _quizDBContext.ExamSessions
                        .Where(s => s.CandidateId == input.CandidateId.ToString())
                        .Where(s => (input.ExamId == null) || (allExamIds == null) || allExamIds.Contains(s.ExamId))
                        .ToListAsync()
                        );

            //if (input.ExamId != null) {
            //    IList<Guid> allExamIds = await _examAppService.GetRecursiveExamIds(
            //                                        new RecursiveExamsRequestDto {
            //                                            ExamId = input.ExamId,
            //                                            MaxDeep = input.MaxDeep }
            //                                        );
            //    _quizDBContext.ExamSessions.RemoveRange(
            //       await _quizDBContext.ExamSessions.Where(s => (s.CandidateId == input.CandidateId.ToString()) && allExamIds.Contains(s.ExamId)).ToListAsync());
            //} else {
            //    _quizDBContext.ExamSessions.RemoveRange(
            //       await _quizDBContext.ExamSessions.Where(s => (s.CandidateId == input.CandidateId.ToString())).ToListAsync());
            //}
            return await _quizDBContext.SaveChangesAsync();
        }

        /// <summary>
        ///     Delete old notes and errors of the passed user on the passed exams
        /// </summary>
        public async Task<int> DeleteCandidateNotes(UserSessionsRequestDto input) {
            IList<Guid> allExamIds = (input.ExamId != null)
                ? await _examAppService.GetRecursiveExamIds(
                            new RecursiveExamsRequestDto(input.ExamId.Value, input.MaxDeep))
                : null;

            _quizDBContext.CandidateNotes.RemoveRange(
                await _quizDBContext.CandidateNotes
                .Where(s => s.CandidateId == input.CandidateId.ToString())
                .Where(s => (input.ExamId == null) || (allExamIds == null) || allExamIds.Contains(s.ExamId))
                .ToListAsync());
            return await _quizDBContext.SaveChangesAsync();
        }

        public Task<int> CountUserSessions(UserSessionsRequestDto input) {
            return _quizDBContext.ExamSessions
                        .WhereIf((input.CandidateId ?? Guid.Empty) != Guid.Empty, s => s.CandidateId == input.CandidateId.ToString())
                        .Where(s => s.IsEnded)
                        .CountAsync();
        }

        public Task<int> CountCandidateNotes(UserSessionsRequestDto input) {
            return _quizDBContext.CandidateNotes
                        .WhereIf((input.CandidateId ?? Guid.Empty) != Guid.Empty, s => s.CandidateId == input.CandidateId.ToString())
                        .CountAsync();
        }

        public Task<int> CountCandidateErrors(UserSessionsRequestDto input) {
            return _quizDBContext.CandidateNotes
                        .WhereIf((input.CandidateId ?? Guid.Empty) != Guid.Empty, s => s.CandidateId == input.CandidateId.ToString())
                        .Where(s => s.NumOfWrongAnswers > 0)
                        .CountAsync();
        }

        public async Task<int> CountQuestionNeverAnswered(UserSessionsRequestDto input) {
            var examIds = await _examAppService.GetRecursiveExamIds(new RecursiveExamsRequestDto(input.ExamId, input.MaxDeep));
            var totalQ = await _quizDBContext.Questions.CountAsync(q => examIds.Contains(q.ExamId));

            var qrySessionIds = _quizDBContext.ExamSessions
                                        .WhereIf((input.CandidateId ?? Guid.Empty) != Guid.Empty, s => s.CandidateId == input.CandidateId.ToString())
                                        .Select(es => es.Id);
            var totalAnswers = await _quizDBContext.ExamSessionItems
                                        .Where(item => qrySessionIds.Contains(item.SessionId))
                                        .Select(item => item.QuestionId)
                                        .Distinct()
                                        .CountAsync();

            return totalQ - totalAnswers;
        }

    }

}
