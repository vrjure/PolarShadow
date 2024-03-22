using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class PreferenceManager
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbContextFactory;
        public PreferenceManager(IDbContextFactory<PolarShadowDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Set(PreferenceEntity item)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            try
            {
                dbContext.Preferences.Update(item);
                dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    entry.State = EntityState.Added;
                }
                dbContext.SaveChanges();
            }
        }

        public async Task SetAsync(PreferenceEntity item)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            try
            {
                dbContext.Preferences.Update(item);
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    entry.State = EntityState.Added;
                }
                await dbContext.SaveChangesAsync();
            }
        }

        public PreferenceEntity Get(string key)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return dbContext.Preferences.Where(f=>f.Key == key).FirstOrDefault();
        }

        public async Task<PreferenceEntity> GetAsync(string key)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Preferences.Where(f => f.Key == key).FirstOrDefaultAsync();
        }

        public void Clear()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Preferences.ExecuteDelete();
        }

        public async Task ClearAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.Preferences.ExecuteDeleteAsync();
        }
    }
}
