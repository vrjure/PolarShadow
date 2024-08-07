using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal class PreferenceService : IPreferenceService
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbContextFactory;
        public PreferenceService(IDbContextFactory<PolarShadowDbContext> dbFactory)
        {
            _dbContextFactory = dbFactory;
        }

        public void Set(PreferenceModel item)
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

        public async Task SetAsync(PreferenceModel item)
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

        public PreferenceModel Get(string key)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return dbContext.Preferences.Where(f => f.Key == key).FirstOrDefault();
        }

        public async Task<PreferenceModel> GetAsync(string key)
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

        public async Task<ICollection<PreferenceModel>> GetAllAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Preferences.ToListAsync();
        }
    }
}
