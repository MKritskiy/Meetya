namespace Application.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(params object[] ids);
    Task<bool> DeleteByIdAsync(params object[] ids);
    Task<int?> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
}
