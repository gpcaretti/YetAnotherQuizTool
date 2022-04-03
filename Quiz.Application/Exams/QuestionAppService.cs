using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PatenteN.Quiz.Domain;
using PatenteN.Quiz.Domain.Exams;

namespace PatenteN.Quiz.Application.Exams {
    public class QuestionAppService : QuizApplicationService<Question, QuestionDto, Guid>, IQuestionAppService {

        public QuestionAppService(QuizDBContext dbContext, IMapper mapper) : base(dbContext, mapper) {
        }

        public async Task<ICollection<QuestionAndChoicesDto>> GetRecursiveQuestionsByExam(QuestionsByExamRequestDto input) {
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
                .Where(q => examIds.Contains(q.ExamId));
            query = (input.IsRandom)
                ? query.OrderBy(q => Guid.NewGuid())
                : !string.IsNullOrEmpty(input.Sorting)
                    ? query.OrderBy(input.Sorting)
                    : query.OrderBy(q => q.ExamId).ThenBy(q => q.Code);
            query = query.AsNoTracking().Skip(input.SkipCount).Take(input.MaxResultCount);

            var entities = await query.ToListAsync();
            return _mapper.Map<QuestionAndChoicesDto[]>(entities);
        }

        public async Task<QnADto> PrepareExamAttempt(QuestionsByExamRequestDto input) {
            // get the exam title/name
            string examName = await _dbContext.Exams.Where(e => e.Id == input.ExamId).Select(o => o.Name).SingleOrDefaultAsync();
            if (string.IsNullOrEmpty(examName) && !await _dbContext.Exams.AnyAsync(e => e.Id == input.ExamId))
                throw new Exception($"Exam not found (id: ${input.ExamId})");
            // get the questions
            var questions = await GetRecursiveQuestionsByExam(input);

            return new QnADto {
                ExamId = input.ExamId,
                ExamName = examName,
                Questions = questions,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        public async Task<QnADto> GetQuestionListByExam(Guid examId) {
            IList<QuestionDetailsDto> qListDto = new List<QuestionDetailsDto>();

            string examName = await _dbContext.Exams.Where(e => e.Id == examId).Select(o => o.Name).SingleOrDefaultAsync();
            if (string.IsNullOrEmpty(examName) && !await _dbContext.Exams.AnyAsync(e => e.Id == examId))
                throw new Exception($"Exam not found (id: ${examId})");

            var questions = await _dbContext.Questions.Where(q => q.ExamId == examId).ToListAsync();
            foreach (var qItem in questions) {
                // get question's details
                IList<OptionDetailsDto> optDetailsDto = new List<OptionDetailsDto>();
                var qDetailsDto = new QuestionDetailsDto {
                    QuestionId = qItem.Id,
                    QuestionText = qItem.Statement,
                };
                // get question answers
                var options = await _dbContext.Choices
                    .Where(q => q.QuestionId == qItem.Id).Select(o => new { OptionId = o.Id, Option = o.Statement })
                    .ToListAsync();

                qDetailsDto.options = options
                    .Select(c => new OptionDetailsDto() {
                                        OptionId = c.OptionId,
                                        Option = c.Option,
                                    })
                    .ToList();

                var ans = await _dbContext.Answers
                    .Where(q => q.QuestionId == qItem.Id).Select(o => new { AnswerId = o.Id, OptionId = o.ChoiceId, Answer = o.Statement, })
                    .FirstOrDefaultAsync();
                qDetailsDto.answer = new AnswerDetailsDto() {
                    AnswerId = ans.AnswerId,
                    OptionId = ans.OptionId,
                    Answer = ans.Answer
                };

                qListDto.Add(qDetailsDto);
            }

            return new QnADto() {
                ExamId = examId,
                ExamName = examName,
                //questionsOLD = qListDto
            };
        }

        public Task<int> UpdateQuestion(Question entity) {
            throw new NotImplementedException();
        }

    }
}
