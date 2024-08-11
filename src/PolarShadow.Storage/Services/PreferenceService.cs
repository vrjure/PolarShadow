using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal class PreferenceService : IDbPreferenceService
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbContextFactory;
        private readonly IMemoryCache _cache;
        public PreferenceService(IDbContextFactory<PolarShadowDbContext> dbFactory, IMemoryCache cahce = null)
        {
            _dbContextFactory = dbFactory;
            _cache = cahce;
        }

        public void Set(PreferenceModel item)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            try
            {
                dbContext.Preferences.Update(item);
                dbContext.SaveChanges();

                _cache?.Set(item.Key, item);
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

                _cache?.Set(item.Key, item);
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
            if (_cache?.TryGetValue(key, out PreferenceModel cache) == true)
            {
                return cache;
            }
            using var dbContext = _dbContextFactory.CreateDbContext();
            return dbContext.Preferences.Where(f => f.Key == key).FirstOrDefault();
        }

        public async Task<PreferenceModel> GetAsync(string key)
        {
            if (_cache?.TryGetValue(key, out PreferenceModel cache) == true)
            {
                return cache;
            }

            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Preferences.Where(f => f.Key == key).FirstOrDefaultAsync();
        }

        public void Clear()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            if (_cache != null)
            {
                var allValue = dbContext.Preferences.ToList();
                dbContext.Preferences.ExecuteDelete();
                foreach (var item in allValue)
                {
                    _cache.Remove(item.Key);
                }
            }
            else
            {
                dbContext.Preferences.ExecuteDelete();
            }
        }

        public async Task ClearAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            if (_cache != null)
            {
                var allValue = await dbContext.Preferences.ToListAsync();
                await dbContext.Preferences.ExecuteDeleteAsync();
                foreach (var item in allValue)
                {
                    _cache.Remove(item.Key);
                }
            }
            else
            {
                await dbContext.Preferences.ExecuteDeleteAsync();
            }
        }

        public async Task<ICollection<PreferenceModel>> GetAllAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Preferences.ToListAsync();
        }
    }
}
