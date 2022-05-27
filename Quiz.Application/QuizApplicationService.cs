using System.Linq.Dynamic.Core;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quiz.Application.Dtos;
using Quiz.Application.Guids;
using Quiz.Domain;

namespace Quiz.Application {
    public class QuizApplicationService<TEntity, TEntityDto, TPrimaryKey>
        : IQuizApplicationService<TEntityDto, TPrimaryKey>
        where TEntity : Entity<TPrimaryKey>
        where TEntityDto : BaseEntityDto<TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey> {

        protected readonly ILogger _logger;
        protected readonly IGuidGenerator GuidGenerator;

        protected readonly QuizDBContext _dbContext;
        protected readonly QuizIdentityDBContext _dbIdentityContext;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly IMapper _mapper;

        public QuizApplicationService(ILogger logger,
            IGuidGenerator guidGenerator,
            QuizDBContext dbContext,
            QuizIdentityDBContext dbIdentityContext,
            IMapper mapper) {
            GuidGenerator = guidGenerator;

            _logger = logger;
            _dbContext = dbContext;
            _dbIdentityContext = dbIdentityContext;
            _mapper = mapper;

            _dbSet = dbContext.Set<TEntity>();
        }

        /// <summary>
        ///     If no entity is found, then null is returned
        /// </summary>
        public virtual async Task<TEntityDto> FindById(TPrimaryKey id) {
            var entity = await _dbSet.FindAsync(id);
            return (entity != null) ? _mapper.Map<TEntityDto>(entity) : default(TEntityDto);
        }

        public async Task<ICollection<TEntityDto>> GetAll(PagedAndSortedResultRequestDto input) {
            IQueryable<TEntity> query = string.IsNullOrEmpty(input.Sorting)
                ? _dbSet.OrderBy(e => e.Id)
                : _dbSet.OrderBy(input.Sorting);
            query = query.Skip(input.SkipCount).Take(input.MaxResultCount);

            var entities = await query.ToListAsync();
            return _mapper.Map<TEntityDto[]>(entities);
        }

        //protected async Task<ICollection<TEntityDto>> Search(Expression<Func<TEntity, bool>> predicate = null, string orderBy = null) {
        //    IQueryable<TEntity> query = _dbSet;
        //    if (predicate != null) query = query.Where(predicate);
        //    if (orderBy != null) query = query.OrderBy(orderBy);

        //    var entities = await query.ToListAsync();
        //    return _mapper.Map<TEntityDto[]>(entities);
        //}

        //public virtual async Task<TEntityDto> FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null) {
        //    IQueryable<TEntity> query = _dbSet;
        //    var entity = await query.FirstOrDefaultAsync(predicate);
        //    return (entity != null) ? _mapper.Map<TEntityDto>(entity) : default(TEntityDto);
        //}

        //public virtual Task<bool> Any(Expression<Func<TEntity, bool>> search = null) {
        //    IQueryable<TEntity> query = _dbSet;
        //    if (search != null) {
        //        query = query.Where(search);
        //    }
        //    return query.AnyAsync();
        //}

        //public virtual Task<int> Count(Expression<Func<TEntity, bool>> search = null) {
        //    IQueryable<TEntity> query = _dbSet;
        //    return (search != null) ? query.CountAsync(search) : query.CountAsync();
        //}

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
