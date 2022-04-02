using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PatenteN.Quiz.Domain;
using PatenteN.Quiz.Domain.Exams;

namespace PatenteN.Quiz.Application.Exams {
    public class QuestionAppService : QuizApplicationService<Question, QuestionDto, Guid>, IQuestionAppService {

        public QuestionAppService(QuizDBContext dbContext, IMapper mapper) : base(dbContext, mapper) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExamId"></param>
        /// <returns></returns>
        public async Task<QnADto> GetQuestionListByExam(Guid ExamId) {
            IList<QuestionDetailsDto> qListDto = new List<QuestionDetailsDto>();

            string examName = await _dbContext.Exams.Where(e => e.Id == ExamId).Select(o => o.Name).SingleOrDefaultAsync();
            var questions = await _dbContext.Questions.Where(q => q.ExamId == ExamId).ToListAsync();
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

                //var ans = await _dbContext.Answers
                //    .Where(q => q.QuestionId == qItem.Id).Select(o => new { AnswerId = o.Id, OptionId = o.ChoiceId, Answer = o.Statement, })
                //    .FirstOrDefaultAsync();
                //qDetailsDto.answer = new AnswerDetailsDto() {
                //    AnswerId = ans.AnswerId,
                //    OptionId = ans.OptionId,
                //    Answer = ans.Answer
                //};

                qListDto.Add(qDetailsDto);
            }

            return new QnADto() {
                ExamId = ExamId,
                Exam = examName,
                questions = qListDto
            };
        }

        public Task<int> UpdateQuestion(Question entity) {
            throw new NotImplementedException();
        }

    }
}
