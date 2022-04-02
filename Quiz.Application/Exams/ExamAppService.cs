using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PatenteN.Quiz.Domain;
using PatenteN.Quiz.Domain.Exams;
using Microsoft.EntityFrameworkCore;

namespace PatenteN.Quiz.Application.Exams {
    public class ExamAppService : QuizApplicationService<Exam, ExamDto, Guid>, IExamAppService {

        public ExamAppService(QuizDBContext dbContext, IMapper mapper) : base(dbContext, mapper) {
        }

        public async Task<ICollection<QuestionDto>> GetQuestionsByExamId(Guid examId) {
            // get the exam ids 
            var qryExamIds1stLevel = _dbSet
                .Where(e => e.Id == examId || e.AncestorId == examId)
                .Select(e => e.Id);
            var examIds = await _dbSet
                .Where(e => qryExamIds1stLevel.Contains(e.Id))
                //.Select(e => new { e.Id, e.Code })
                .Select(e => e.Id)
                .ToListAsync();

            // if there are no exam ids to select, return an empty list
            if (!examIds.Any()) return new QuestionDto[0];

            // run the query over the questions to get all with the proper examm id
            var query = _dbContext.Questions //.Set<Question>()
                .Where(q => examIds.Contains(q.Id))
                .OrderBy(q => q.ExamId)
                .ThenBy(q => q.Code);

            var entities = await query.ToListAsync();
            return _mapper.Map<QuestionDto[]>(entities);
        }

        public async Task<int> UpdateExam(Exam entity) {
            int output = 0;
            _dbSet.Update(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }

    }
}
