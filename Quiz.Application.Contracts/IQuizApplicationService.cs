using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace PatenteN.Quiz.Application {
    public interface IQuizApplicationService<TEntity, TEntityDto> {
        Task<TEntityDto> FindById(int id);
        Task<TEntityDto> FirstOrDefault(Expression<Func<TEntity, bool>> search = null);
        Task<ICollection<TEntityDto>> Search(Expression<Func<TEntity, bool>> search = null);

        Task<bool> Any(Expression<Func<TEntity, bool>> search = null);
        Task<int> Count(Expression<Func<TEntity, bool>> search = null);

        Task<int> Create(TEntityDto dto);
        Task<int> Delete(int id);
    }
}
