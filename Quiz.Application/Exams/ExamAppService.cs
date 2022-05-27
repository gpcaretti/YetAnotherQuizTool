using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quiz.Application.Guids;
using Quiz.Domain;
using Quiz.Domain.Exams;

namespace Quiz.Application.Exams {

    [Authorize(Roles = $"{QuizConstants.Roles.Candidate}, {QuizConstants.Roles.Manager}, {QuizConstants.Roles.Admin}")]
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

        public async Task<IList<ExamDto>> GetAllRootExams() {
            List<Exam> exams = await _dbSet.Where(ex => ex.AncestorId == null).OrderBy(ex => ex.Code).ToListAsync();
            return _mapper.Map<List<ExamDto>>(exams);
        }

        /// <summary>
        ///     Get all children of the passed exam Id up to the passed max depth
        /// </summary>
        public Task<IList<Guid>> GetRecursiveExamIds(RecursiveExamsRequestDto input) {
            // take the group of exams from which to retrieve the questions
            return GetRecursiveExamIds(_dbSet.Where(ex => ex.Id == input.ExamId), input.MaxDeep);
        }

        private async Task<IList<Guid>> GetRecursiveExamIds(IQueryable<Exam> exams, int maxDeep = 100) {
            var result = new List<Guid>(await exams.Select(ex => ex.Id).Distinct().ToListAsync());
            if (maxDeep > 0) {
                var qry = _dbContext.Exams.Where(ex => ex.AncestorId != null && exams.Select(exx => exx.Id).Contains(ex.AncestorId.Value));
                if (qry.Any()) {
                    result.AddRange(await GetRecursiveExamIds(qry, --maxDeep));
                }
            }
            return result.Distinct().ToList();
        }

        //private async Task<IList<Guid>> GetRecursiveExamIds2(IQueryable<Exam> exams, int maxDeep = 100) {
        //    var result = new List<Guid>(await exams.Select(ex => ex.Id).Distinct().ToListAsync());
        //    if (maxDeep > 0) {
        //        var qry = _dbContext.Exams.Where(ex => ex.AncestorId != null && exams.Select(exx => exx.Id).Contains(ex.AncestorId.Value));
        //        if (qry.Any()) {
        //            result.AddRange(await GetRecursiveExamIds(qry, --maxDeep));
        //        }
        //    }
        //    return result.Distinct().ToList();
        //}
    }
}
