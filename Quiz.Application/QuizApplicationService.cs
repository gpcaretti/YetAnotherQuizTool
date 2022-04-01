using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PatenteN.Quiz.Domain;
using PatenteN.Quiz.Domain.Users;

namespace PatenteN.Quiz.Application {
    public class QuizApplicationService<TEntity, TEntityDto> : IQuizApplicationService<TEntity, TEntityDto> where TEntity : BaseEntity {

        protected readonly QuizDBContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly IMapper _mapper;

        public QuizApplicationService(QuizDBContext dbContext, IMapper mapper) {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
            _mapper = mapper;
        }

        public virtual async Task<TEntityDto> FindById(int id) {
            var entity = await _dbSet.FindAsync(id);
            return (entity != null) ? _mapper.Map<TEntityDto>(entity) : default(TEntityDto);
        }

        public virtual async Task<TEntityDto> FirstOrDefault(Expression<Func<TEntity, bool>> search = null) {
            IQueryable<TEntity> query = _dbSet;
            var entity = await query.FirstOrDefaultAsync(search);
            return (entity != null) ? _mapper.Map<TEntityDto>(entity) : default(TEntityDto);
        }

        public async Task<ICollection<TEntityDto>> Search(Expression<Func<TEntity, bool>> search = null) {
            IQueryable<TEntity> query = _dbSet;
            if (search != null) {
                query = query.Where(search);
            }
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
            entity.CreatedOn = DateTime.Now;

            _dbSet.Add(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> Delete(int id) {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return 0;
            _dbSet.Remove(entity);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
