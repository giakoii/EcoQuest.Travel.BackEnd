using BackEnd.SystemClient;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models.Helpers;

public class AppDbContext : EcoQuestTravelContext
{
    public NLog.Logger _Logger;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(new DbContextOptions<EcoQuestTravelContext>())
    {
    }
    
    /// <summary>
    /// Save changes async with common value
    /// </summary>
    /// <param name="updateUserId"></param>
    /// <param name="needLogicalDelete"></param>
    /// <returns></returns>
    public Task<int> SaveChangesAsync(string updateUserId, bool needLogicalDelete = false)
    {
        this.SetCommonValue(updateUserId, needLogicalDelete);
        return base.SaveChangesAsync();
    }
    
    /// <summary>
    /// Save changes with common value
    /// </summary>
    /// <param name="updateUserId"></param>
    /// <param name="needLogicalDelete"></param>
    /// <returns></returns>
    public int SaveChanges(string updateUserId, bool needLogicalDelete = false)
    {
        this.SetCommonValue(updateUserId, needLogicalDelete);
        return base.SaveChanges();
    }
    
    /// <summary>
    /// Set common value for all Entities
    /// </summary>
    /// <param name="updateUser"></param>
    /// <param name="needLogicalDelete"></param>
    private void SetCommonValue(string updateUser, bool needLogicalDelete = false)
    {
        // Register
        var newEntities = this.ChangeTracker.Entries()
            .Where(
                x => x.State == EntityState.Added &&
                x.Entity != null
                )
            .Select(e => e.Entity);

        // Update
        var modifiedEntities = this.ChangeTracker.Entries()
            .Where(
                x => x.State == EntityState.Modified &&
                    x.Entity != null
                    )
                .Select(e => e.Entity);

        // Get current time
        var now = DateTime.UtcNow;
        // Add
        foreach (dynamic newEntity in newEntities)
        {
            try
            {
                newEntity.IsActive = true;
                newEntity.CreatedAt = now;
                newEntity.CreatedBy = updateUser;
                newEntity.UpdatedBy = updateUser;
                newEntity.UpdatedAt = now;
            }
            catch (IOException e)
            {
                // There may be no elements, so don't throw an error
                _Logger.Error(e);
            }
        }

        // Set modifiedEntities
        foreach (dynamic modifiedEntity in modifiedEntities)
        {
            try
            {
                if (needLogicalDelete)
                {
                    // Delete
                    modifiedEntity.IsActive = false;
                    modifiedEntity.UpdatedBy = updateUser;
                }
                else
                {
                    // Normal
                    modifiedEntity.IsActive = true;
                    modifiedEntity.UpdatedBy = updateUser;
                }
                modifiedEntity.UpdatedAt = now;
            }
            catch (IOException e)
            {
                // There may be no elements, so don't throw an error
                _Logger.Error(e);
            }
        }

    }
}