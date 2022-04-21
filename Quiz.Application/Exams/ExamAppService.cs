using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quiz.Application.Exams.Sessions;
using Quiz.Application.Guids;
using Quiz.Application.Users;
using Quiz.Domain;
using Quiz.Domain.Exams;
using Quiz.Domain.Exams.Sessions;

namespace Quiz.Application.Exams {
    public class ExamAppService : QuizApplicationService<Exam, ExamDto, Guid>, IExamAppService {

        private readonly IQuestionAppService _questionAppService;

        public ExamAppService(
            ILogger<ExamAppService> logger,
            IGuidGenerator guidGenerator,
            QuizDBContext dbContext,
            IMapper mapper,
            IQuestionAppService questionAppService) : base(logger, guidGenerator, dbContext, mapper) {
            _questionAppService = questionAppService;
        }

        public async Task<PrepareExamSessionResponseDto> PrepareExamSession(PrepareExamSessionRequestDto input, BasicCandidateDto candidate = null) {
            // get the exam title/name
            var exam = await _dbSet.FirstOrDefaultAsync(e => e.Id == input.ExamId);
            if (exam == null) throw new Exception($"Exam not found (id: ${input.ExamId})");

            // get the questions
            var questions = await _questionAppService.GetRecursiveQuestionsByExam(input, candidate);

            return new PrepareExamSessionResponseDto {
                ExamId = input.ExamId,
                Name = exam.Name,
                Duration = exam.Duration,
                Questions = questions,
            };
        }

        /// <summary>
        ///     Save a user Session
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Guid> SaveUserSession(ExamSessionResultsRequestDto input, [NotNull] BasicCandidateDto candidate) {
            // create an exam session
            var session = new ExamSession(GuidGenerator.Create());
            _mapper.Map(input, session);
            session.ExecutedOn = DateTimeOffset.Now;
            session.CandidateId = await _dbContext.Candidates.Where(u => u.Id == candidate.Id).Select(u => u.Id).SingleAsync();
            session.ExamName = await _dbContext.Exams.Where(ex => ex.Id == input.ExamId).Select(ex => ex.Name).SingleAsync();
            session.NumOfQuestions = input.Answers.Count;
            session.NumOfCorrectAnswers = input.Answers.Count(ans => ans.IsCorrect);
            session.NumOfWrongAnswers = input.Answers.Count(ans => !ans.IsCorrect && ans.UserChoiceId.HasValue);
            session.QSequence = String.Join(',', input.Answers.Select(ans => ans.QuestionId.ToString("D")).ToArray());
            _dbContext.ExamSessions.Add(session);

            // add the user's answers
            foreach (var dto in input.Answers) {
                var ans = new ExamSessionItem(GuidGenerator.Create(), session.Id);
                _mapper.Map(dto, ans);
                _dbContext.ExamSessionItems.Add(ans);
            }

            // add the user's errors, doubts, etc
            var candidateNotes = await _dbContext.CandidateNotes.Where(cn => cn.CandidateId == candidate.Id).ToListAsync();
            foreach (var answDto in input.Answers) {
                // skip unanswered, if requested
                if (input.SkipUnanswered && !answDto.UserChoiceId.HasValue) continue;

                var cn = candidateNotes.FirstOrDefault(cn => cn.QuestionId == answDto.QuestionId) ?? new CandidateNote(candidate.Id, answDto.QuestionId, answDto.ExamId);
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
                }
            }

            //// delete all unseless notes
            //_dbContext.CandidateNotes.RemoveRange(_dbContext.CandidateNotes.Where(cn => (cn.NumOrWrongAnswers <= 0) && !cn.IsMarkedAsDoubt && !cn.IsMarkedAsHidden);

            // commit all
            int nSaved = await _dbContext.SaveChangesAsync();
            if (nSaved <= 0) throw new Exception();
            return session.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateExam(Exam entity) {
            int output = 0;
            _dbSet.Update(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }

    }
}
