using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PatenteN.Quiz.Domain;
using PatenteN.Quiz.Domain.Exams;

namespace PatenteN.Quiz.Application.Exams {
    public class QuestionAppService : QuizApplicationService<Question, QuestionDto>, IQuestionAppService {

        public QuestionAppService(QuizDBContext dbContext, IMapper mapper) : base(dbContext, mapper) {
        }

        public async Task<QnADto> GetQuestionList(int ExamID) {
            IList<QuestionDetailsDto> qListDto = new List<QuestionDetailsDto>();

            string examName = await _dbContext.Exam.Where(e => e.ExamID == ExamID).Select(o => o.Name).SingleOrDefaultAsync();
            var questions = await _dbContext.Question.Where(q => q.ExamID == ExamID).ToListAsync();
            foreach (var qItem in questions) {
                IList<OptionDetailsDto> optDetailsDto = new List<OptionDetailsDto>();
                var qDetailsDto = new QuestionDetailsDto();
                qDetailsDto.QuestionID = qItem.QuestionID;
                qDetailsDto.QuestionType = qItem.QuestionType;
                qDetailsDto.QuestionText = qItem.DisplayText;

                var options = await _dbContext.Choice
                    .Where(q => q.QuestionID == qItem.QuestionID).Select(o => new { OptionID = o.ChoiceID, Option = o.DisplayText })
                    .ToListAsync();

                foreach (var optItem in options) {
                    var optDto = new OptionDetailsDto() {
                        OptionID = optItem.OptionID,
                        Option = optItem.Option
                    };
                    optDetailsDto.Add(optDto);
                }
                qDetailsDto.options = optDetailsDto;

                var ans = await _dbContext.Answer
                    .Where(q => q.QuestionID == qItem.QuestionID).Select(o => new { AnswerID = o.Sl_No, OptionID = o.ChoiceID, Answer = o.DisplayText, })
                    .FirstOrDefaultAsync();
                qDetailsDto.answer = new AnswerDetailsDto() {
                    AnswarID = ans.AnswerID,
                    OptionID = ans.OptionID,
                    Answar = ans.Answer
                };

                qListDto.Add(qDetailsDto);
            }

            return new QnADto() {
                ExamID = ExamID,
                Exam = examName,
                questions = qListDto
            };
        }

        public Task<int> UpdateQuestion(Question entity) {
            throw new NotImplementedException();
        }

    }
}
