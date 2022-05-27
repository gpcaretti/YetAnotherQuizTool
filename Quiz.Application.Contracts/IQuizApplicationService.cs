using Quiz.Application.Dtos;

namespace Quiz.Application {

    public interface IQuizApplicationService<TEntityDto, TPrimaryKey>
        where TEntityDto : BaseEntityDto<TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey> {

        /// <summary>
        ///     If no entity is found, then null is returned
        /// </summary>
        Task<TEntityDto> FindById(TPrimaryKey id);

        Task<ICollection<TEntityDto>> GetAll(PagedAndSortedResultRequestDto input);
        //Task<TEntityDto> FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null);
        //Task<bool> Any(Expression<Func<TEntity, bool>> predicate = null);
        //Task<int> Count(Expression<Func<TEntity, bool>> predicate = null);

        Task<int> Create(TEntityDto dto);
        Task<int> Delete(TPrimaryKey id);

    }

}
