using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quiz.Application.Guids;
using Quiz.Domain;

namespace Quiz.Application {
    public class QuizApplicationService<TEntity, TEntityDto, TPrimaryKey>
        : IQuizApplicationService<TEntity, TEntityDto, TPrimaryKey>
        where TEntity : Entity<TPrimaryKey> {

        protected readonly ILogger _logger;
        protected readonly IGuidGenerator GuidGenerator;

        protected readonly QuizDBContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly IMapper _mapper;

        public QuizApplicationService(ILogger logger, IGuidGenerator guidGenerator, QuizDBContext dbContext, IMapper mapper) {
            _logger = logger;
            GuidGenerator = guidGenerator;
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
            _mapper = mapper;
        }

        public virtual async Task<TEntityDto> FindById(TPrimaryKey id) {
            var entity = await _dbSet.FindAsync(id);
            return (entity != null) ? _mapper.Map<TEntityDto>(entity) : default(TEntityDto);
        }

        public virtual async Task<TEntityDto> FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null) {
            IQueryable<TEntity> query = _dbSet;
            var entity = await query.FirstOrDefaultAsync(predicate);
            return (entity != null) ? _mapper.Map<TEntityDto>(entity) : default(TEntityDto);
        }

        public async Task<ICollection<TEntityDto>> Search(Expression<Func<TEntity, bool>> predicate = null, string orderBy = null) {
            IQueryable<TEntity> query = _dbSet;
            if (predicate != null) query = query.Where(predicate);
            if (orderBy != null) query = query.OrderBy(orderBy);

            var entities = await query.ToListAsync();
            return _mapper.Map<TEntityDto[]>(entities);
        }

        public virtual Task<bool> Any(Expression<Func<TEntity, bool>> search = null) {
            IQueryable<TEntity> query = _dbSet;
            if (search != null) {
                query = query.Where(search);
            }
            return query.AnyAsync();
        }

        public virtual Task<int> Count(Expression<Func<TEntity, bool>> search = null) {
            IQueryable<TEntity> query = _dbSet;
            return (search != null) ? query.CountAsync(search) : query.CountAsync();
        }

        public virtual async Task<int> Create(TEntityDto dto) {
            TEntity entity = _mapper.Map<TEntityDto, TEntity>(dto);
            entity.CreatedOn = DateTimeOffset.Now;

            _dbSet.Add(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> Delete(TPrimaryKey id) {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return 0;
            _dbSet.Remove(entity);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
