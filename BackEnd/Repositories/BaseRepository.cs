using System.Linq.Expressions;
using BackEnd.Models.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories;

public class BaseRepository<TEntity, TType>(AppDbContext context) : IBaseRepository<TEntity, TType>
    where TEntity : class
{
    protected readonly AppDbContext Context = context;

    private DbSet<TEntity> DbSet => Context.Set<TEntity>();

    /// <summary>
    /// Find multiple records - Use for select
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="isTracking"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    public IQueryable<TEntity?> Find(Expression<Func<TEntity, bool>> predicate = null, bool isTracking = false,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = DbSet;

        query = query.Where(predicate);

        query = includes.Aggregate(query, (current, inc) => current.Include(inc));

        if (!isTracking) query = query.AsNoTracking();

        return query;
    }
    
    /// <summary>
    /// Get all data from any view mapped to a class
    /// </summary>
    /// <typeparam name="TViewEntity">View entity class</typeparam>
    /// <returns></returns>
    public IQueryable<TViewEntity> GetView<TViewEntity>() where TViewEntity : class
    {
        return Context.Set<TViewEntity>().AsNoTracking();
    }

    /// <summary>
    /// Get view data with condition
    /// </summary>
    /// <typeparam name="TViewEntity"></typeparam>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IQueryable<TViewEntity> GetView<TViewEntity>(Expression<Func<TViewEntity, bool>> predicate) where TViewEntity : class
    {
        return Context.Set<TViewEntity>().AsNoTracking().Where(predicate);
    }

    /// <summary>
    /// Get entity by id - Use for Insert, Update, Delete
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TEntity? GetById(TType id)
    {
        return DbSet.Find(id);
    }

    /// <summary>
    /// Get entity by id asynchronously - Use for Insert, Update, Delete
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private async Task<TEntity?> GetByIdAsync(TType id)
    {
        return await DbSet.FindAsync(id);
    }

    /// <summary>
    /// Add entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool Add(TEntity entity)
    {
        Context.Add(entity);
        return true;
    }

    /// <summary>
    /// Add entity asynchronously
    /// </summary>
    public async Task AddAsync(TEntity entity)
    {
        await Context.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
       await Context.AddRangeAsync(entities);
    }

    /// <summary>
    /// Update entity
    /// </summary>
    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    /// <summary>
    /// Update entity asynchronously
    /// </summary>
    public Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Delete entity by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool DeleteById(TType id)
    {
        var entity = GetById(id);
        if (entity == null) return false;

        DbSet.Update(entity);
        return true;
    }

    /// <summary>
    /// Delete entity by ID asynchronously
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteByIdAsync(TType id)
    {
        var entity = await GetByIdAsync(id);

        // Check if entity is null
        if (entity == null) return false;

        DbSet.Update(entity);
        return true;
    }

    /// <summary>
    /// Save changes
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="needLogicalDelete"></param>
    /// <exception cref="NotImplementedException"></exception>
    public int SaveChanges(string userName, bool needLogicalDelete = false)
    {
        return Context.SaveChanges(userName, needLogicalDelete);
    }

    /// <summary>
    /// Save changes asynchronously
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="needLogicalDelete"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<int> SaveChangesAsync(string userName, bool needLogicalDelete = false)
    {
        return await Context.SaveChangesAsync(userName, needLogicalDelete);
    }

    /// <summary>
    /// Execute multiple operations within a transaction.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public void ExecuteInTransaction(Func<bool> action)
    {
        // Begin transaction
        using var transaction = Context.Database.BeginTransaction();
        try
        {
            // Execute action
            if (action())
            {
                transaction.Commit();
            }
            else
            {
                transaction.Rollback();
            }
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Execute multiple operations within a transaction asynchronously.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task ExecuteInTransactionAsync(Func<Task<bool>> action)
    {
        // Begin transaction
        await using var transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            // Execute action
            if (await action())
            {
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}