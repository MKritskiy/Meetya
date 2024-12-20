using System.Linq.Expressions;

namespace Application.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(params object[] ids);
    Task<T?> GetByIdAsync(string includeProperties = "", params object[] ids);

    Task<bool> DeleteByIdAsync(params object[] ids);
    Task<int?> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int? page = null,
            int? pageSize = null);
}
