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

        public ExamAppService(
            ILogger<ExamAppService> logger,
            IGuidGenerator guidGenerator,
            QuizDBContext dbContext,
            QuizIdentityDBContext dbIdentityContext,
            IMapper mapper) : base(logger, guidGenerator, dbContext, dbIdentityContext, mapper) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="EntityNotFoundException">In case the entity to update does not exist</exception>
        public async Task<int> UpdateExam(UpdateExamDto input) {
            var entity = await _dbSet.FirstOrDefaultAsync(ex => ex.Id == input.Id);
            if (entity == null) throw new EntityNotFoundException(type: typeof(Exam), id: input.Id);
            if (entity.IsDeleted.GetValueOrDefault(false)) throw new BusinessException("You are not allowed to modify a deleted entity");

            _mapper.Map(input, entity);
            entity.ModifiedOn = DateTimeOffset.Now;

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
            return DoGetRecursiveExamIds(_dbSet.Where(ex => ex.Id == input.ExamId), input.MaxDeep);
        }

        private async Task<IList<Guid>> DoGetRecursiveExamIds(IQueryable<Exam> exams, int maxDeep = 100) {
            var result = new List<Guid>(await exams.Select(ex => ex.Id).Distinct().ToListAsync());
            if (maxDeep > 0) {
                var qry = _dbContext.Exams.Where(ex => ex.AncestorId != null && exams.Select(exx => exx.Id).Contains(ex.AncestorId.Value));
                if (qry.Any()) {
                    result.AddRange(await DoGetRecursiveExamIds(qry, --maxDeep));
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
