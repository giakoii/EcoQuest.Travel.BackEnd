using System.Linq.Expressions;

namespace BackEnd.Repositories;

public interface IBaseRepository<T, TU> where T : class
{
    // Get multiple records or one record (synchronous)
    IQueryable<T?> Find(Expression<Func<T, bool>> predicate = null!, 
        bool isTracking = true,  
        params Expression<Func<T, object>>[] includes);

    IQueryable<TViewEntity> GetView<TViewEntity>() where TViewEntity : class;

    IQueryable<TViewEntity> GetView<TViewEntity>(Expression<Func<TViewEntity, bool>> predicate = null) where TViewEntity : class;
    
    // Get multiple records or one record (asynchronous)
    T? GetById(TU id);

    // Add entity (synchronous)
    bool Add(T entity);

    // Add entity (asynchronous)
    Task AddAsync(T entity);
    
    Task AddRangeAsync(IEnumerable<T> entities);

    // Update entity (synchronous)
    void Update(T entity);

    // Update entity (asynchronous)
    Task UpdateAsync(T entity);

    // Delete entity by ID (synchronous)
    bool DeleteById(TU id);

    // Delete entity by ID (asynchronous)
    Task<bool> DeleteByIdAsync(TU id);

    int SaveChanges(string userName, bool needLogicalDelete = false);
    
    Task<int> SaveChangesAsync(string userName, bool needLogicalDelete = false);

    public void ExecuteInTransaction(Func<bool> action);

    public Task ExecuteInTransactionAsync(Func<Task<bool>> action);
}
