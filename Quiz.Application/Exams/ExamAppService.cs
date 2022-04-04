using System;
using System.Threading.Tasks;
using AutoMapper;
using Quiz.Domain;
using Quiz.Domain.Exams;

namespace Quiz.Application.Exams {
    public class ExamAppService : QuizApplicationService<Exam, ExamDto, Guid>, IExamAppService {

        public ExamAppService(QuizDBContext dbContext, IMapper mapper) : base(dbContext, mapper) {
        }

        public async Task<int> UpdateExam(Exam entity) {
            int output = 0;
            _dbSet.Update(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }

    }
}
