using System.Linq.Expressions;
using PatenteN.Quiz.Domain;

namespace PatenteN.Quiz.Application {
    public interface IQuizApplicationService<TEntity, TEntityDto, TPrimaryKey>
        where TEntity : BaseEntity<TPrimaryKey> {

        Task<TEntityDto> FindById(TPrimaryKey id);
        Task<TEntityDto> FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null);
        Task<ICollection<TEntityDto>> Search(Expression<Func<TEntity, bool>> predicate = null, string orderBy = null);

        Task<bool> Any(Expression<Func<TEntity, bool>> predicate = null);
        Task<int> Count(Expression<Func<TEntity, bool>> predicate = null);

        Task<int> Create(TEntityDto dto);
        Task<int> Delete(TPrimaryKey id);
    }
}
