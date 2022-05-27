using System.Linq.Expressions;
using Quiz.Application.Dtos;
using Quiz.Application.Sessions;
using Quiz.Domain;

namespace Quiz.Application {

    public interface IQuizApplicationService<TEntity, TEntityDto, TPrimaryKey>
        : IQuizApplicationBaseService<TEntity, TEntityDto, TPrimaryKey>
        where TEntity : Entity<TPrimaryKey> {

        // Change the other one below
    }

    public interface IQuizApplicationBaseService<TEntity, TEntityDto, TPrimaryKey> {
        Task<TEntityDto> FindById(TPrimaryKey id);
        Task<TEntityDto> FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null);

        Task<ICollection<TEntityDto>> GetAll(PagedAndSortedResultRequestDto input);
        Task<bool> Any(Expression<Func<TEntity, bool>> predicate = null);
        Task<int> Count(Expression<Func<TEntity, bool>> predicate = null);

        Task<int> Create(TEntityDto dto);
        Task<int> Delete(TPrimaryKey id);

    }

}
