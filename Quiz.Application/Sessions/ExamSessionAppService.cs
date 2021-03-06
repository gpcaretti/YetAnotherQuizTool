using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quiz.Application.Exams;
using Quiz.Application.Guids;
using Quiz.Domain;
using Quiz.Domain.Exams.Sessions;
using Quiz.Domain.Extensions;

namespace Quiz.Application.Sessions {

    [Authorize(Roles = $"{QuizConstants.Roles.Candidate}, {QuizConstants.Roles.Manager}, {QuizConstants.Roles.Admin}")]
    public class ExamSessionAppService : QuizApplicationService<ExamSession, ExamSessionDto, Guid>, IExamSessionAppService {

        private readonly IExamAppService _examAppService;
        private readonly IQuestionAppService _questionAppService;

        public ExamSessionAppService(
            ILogger<ExamAppService> logger,
            IGuidGenerator guidGenerator,
            QuizDBContext dbContext,
            QuizIdentityDBContext dbIdentityContext,
            IMapper mapper,
            IQuestionAppService questionAppService,
            IExamAppService examAppService) :
            base(logger, guidGenerator, dbContext, dbIdentityContext, mapper) {
            _questionAppService = questionAppService;
            _examAppService = examAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PrepareExamSessionResponseDto> PrepareExamSession(PrepareExamSessionRequestDto input) {
            try {
                // get the exam title/name
                var exam = await _dbContext.Exams.FirstOrDefaultAsync(e => e.Id == input.ExamId);
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
        ///     Save a user Session
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Guid> SaveUserSession(ExamSessionResultsRequestDto input) {
            // validate input params
            if (!await _dbIdentityContext.Users.AnyAsync(u => u.Id == input.CandidateId.ToString()))
                throw new ArgumentException("Cannot find the indicated candidate", nameof(input.CandidateId));
            if (!await _dbContext.Exams.AnyAsync(ex => ex.Id == input.ExamId))
                throw new ArgumentException("Cannot find the indicated exam", nameof(input.ExamId));

            // create the exam session
            var session = new ExamSession(GuidGenerator.Create(), input.CandidateId.Value, input.ExamId);
            _mapper.Map(input, session);
            session.ExecutedOn = DateTimeOffset.Now;
            session.NumOfQuestions = input.Answers.Count;
            session.NumOfCorrectAnswers = input.Answers.Count(ans => ans.IsCorrect);
            session.NumOfWrongAnswers = input.Answers.Count(ans => !ans.IsCorrect && ans.UserChoiceId.HasValue);
            session.QSequence = String.Join(',', input.Answers.Select(ans => ans.QuestionId.Value.ToString("D")).ToArray());
            session.ExamName = await _dbContext.Exams.Where(ex => ex.Id == input.ExamId).Select(ex => ex.Name).SingleAsync();

            _dbContext.ExamSessions.Add(session);

            // add the user's answers
            foreach (var dto in input.Answers) {
                var ans = new ExamSessionItem(GuidGenerator.Create(), session.Id);
                _mapper.Map(dto, ans);
                _dbContext.ExamSessionItems.Add(ans);
            }

            // add the user's errors, doubts, etc
            var candidateNotes = await _dbContext.CandidateNotes.Where(cn => cn.CandidateId == input.CandidateId.ToString()).ToListAsync();
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
                var isNewCn = (_dbContext.Entry(cn).State == EntityState.Detached);

                // check if it is required to be removed
                if (!isNewCn && (cn.NumOfWrongAnswers <= 0) && !cn.IsMarkedAsDoubt && !cn.IsMarkedAsHidden) {
                    // delete it
                    _dbContext.CandidateNotes.Remove(cn);
                } else if (isNewCn && (cn.NumOfWrongAnswers > 0 || cn.IsMarkedAsDoubt || cn.IsMarkedAsHidden)) {
                    // add it
                    _dbContext.CandidateNotes.Add(cn);
                    candidateNotes.Add(cn);
                }
            }

            //// delete all unseless notes
            //_dbContext.CandidateNotes.RemoveRange(_dbContext.CandidateNotes.Where(cn => (cn.NumOrWrongAnswers <= 0) && !cn.IsMarkedAsDoubt && !cn.IsMarkedAsHidden);

            // commit all
            int nSaved = await _dbContext.SaveChangesAsync();
            if (nSaved <= 0) throw new Exception();
            return session.Id;
        }

        public async Task<IList<SessionsStatisticsDto>> GetUserStats(UserSessionsRequestDto input) {
            var rootExams = input.ExamId.HasValue
                ? (new ExamDto[] { await _examAppService.FindById(input.ExamId.Value) })
                : await _examAppService.GetAllRootExams();
            if (rootExams.Count <= 0) return new List<SessionsStatisticsDto>();

            var candidateName = input.CandidateId.HasValue
                ? await _dbIdentityContext.Users
                            .Where(u => u.Id == input.CandidateId.ToString())
                            .Select(u => u.UserName)
                            .FirstOrDefaultAsync()
                : null;

            // FIXME too much results (mainly if CandidateId is null)
            var candidateSessions = await _dbContext.ExamSessions
                                                .WhereIf(input.CandidateId.HasValue, cd => cd.CandidateId == input.CandidateId.ToString())
                                                .Where(cd => cd.IsEnded)
                                                .ToListAsync();

            var output = new List<SessionsStatisticsDto>(rootExams.Count);
            foreach (var exam in rootExams) {
                var allExamIds = await _examAppService.GetRecursiveExamIds(new RecursiveExamsRequestDto { ExamId = exam.Id, MaxDeep = 10 });
                var stat = new SessionsStatisticsDto {
                    CandidateId = input.CandidateId,
                    CandidateName = candidateName,
                    ExamName = exam.Name,
                    NumOfAvailableQuestions = await _dbContext.Questions.CountAsync(q => allExamIds.Contains(q.ExamId)),
                    NumOfCarriedOutSessions = candidateSessions.Count(cs => allExamIds.Contains(cs.ExamId)),
                    NumOfWrongAnswers = await _dbContext.CandidateNotes.CountAsync(cn => allExamIds.Contains(cn.ExamId) &&  (cn.CandidateId == input.CandidateId.ToString()) && (cn.NumOfWrongAnswers > 0)),
                    NumOfDoubtAnswers = await _dbContext.CandidateNotes.CountAsync(cn => allExamIds.Contains(cn.ExamId) &&  (cn.CandidateId == input.CandidateId.ToString()) && cn.IsMarkedAsDoubt),
                };
                stat.NumOfNeverAnswered =
                    stat.NumOfAvailableQuestions -
                    await _dbContext.ExamSessionItems
                            .Where(item => candidateSessions.Select(cs => cs.Id).Contains(item.SessionId) && item.IsAnswered)
                            .Select(item => item.QuestionId)
                            .Distinct()
                            .CountAsync();

                output.Add(stat);
            }

            return output;
        }

        public async Task<int> DeleteUserSessions(UserSessionsRequestDto input) {
            _dbContext.ExamSessions.RemoveRange(
               await _dbContext.ExamSessions.Where(s => (s.CandidateId == input.CandidateId.ToString())).ToListAsync());
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteCandidateNotes(UserSessionsRequestDto input) {
            _dbContext.CandidateNotes.RemoveRange(
                await _dbContext.CandidateNotes.Where(s => (s.CandidateId == input.CandidateId.ToString())).ToListAsync());
            return await _dbContext.SaveChangesAsync();
        }

        public Task<int> CountUserSessions(UserSessionsRequestDto input) {
            return _dbContext.ExamSessions
                        .WhereIf((input.CandidateId ?? Guid.Empty) != Guid.Empty, s => s.CandidateId == input.CandidateId.ToString())
                        .Where(s => s.IsEnded)
                        .CountAsync();
        }

        public Task<int> CountCandidateNotes(UserSessionsRequestDto input) {
            return _dbContext.CandidateNotes
                        .WhereIf((input.CandidateId ?? Guid.Empty) != Guid.Empty, s => s.CandidateId == input.CandidateId.ToString())
                        .CountAsync();
        }

        public Task<int> CountCandidateErrors(UserSessionsRequestDto input) {
            return _dbContext.CandidateNotes
                        .WhereIf((input.CandidateId ?? Guid.Empty) != Guid.Empty, s => s.CandidateId == input.CandidateId.ToString())
                        .Where(s => s.NumOfWrongAnswers > 0)
                        .CountAsync();
        }

        public async Task<int> CountQuestionNeverAnswered(UserSessionsRequestDto input) {
            var examIds = await _examAppService.GetRecursiveExamIds(new RecursiveExamsRequestDto { ExamId = input.ExamId, MaxDeep = 10 });
            var totalQ = await _dbContext.Questions.CountAsync(q => examIds.Contains(q.ExamId));

            var qrySessionIds = _dbContext.ExamSessions
                                        .WhereIf((input.CandidateId ?? Guid.Empty) != Guid.Empty, s => s.CandidateId == input.CandidateId.ToString())
                                        .Select(es => es.Id);
            var totalAnswers = await _dbContext.ExamSessionItems
                                        .Where(item => qrySessionIds.Contains(item.SessionId))
                                        .Select(item => item.QuestionId)
                                        .Distinct()
                                        .CountAsync();

            return totalQ - totalAnswers;
        }

    }

}
