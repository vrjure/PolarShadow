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

            if (dbContext.Preferences.Any(f=>f.Key == item.Key))
            {
                dbContext.Preferences.Update(item);
            }
            else
            {
                dbContext.Preferences.Add(item);
            }

            dbContext.SaveChanges();

            _cache?.Set(item.Key, item);
        }

        public async Task SetAsync(PreferenceModel item)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            if (await dbContext.Preferences.AnyAsync(f=>f.Key == item.Key))
            {
                dbContext.Preferences.Update(item);
            }
            else
            {
                dbContext.Preferences.Add(item);
            }
            await dbContext.SaveChangesAsync();

            _cache?.Set(item.Key, item);
        }

        public PreferenceModel Get(string key)
        {
            if (_cache?.TryGetValue(key, out PreferenceModel cache) == true)
            {
                return cache;
            }
            using var dbContext = _dbContextFactory.CreateDbContext();
            return dbContext.Preferences.Where(f => f.Key == key).AsNoTracking().FirstOrDefault();
        }

        public async Task<PreferenceModel> GetAsync(string key)
        {
            if (_cache?.TryGetValue(key, out PreferenceModel cache) == true)
            {
                return cache;
            }

            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Preferences.Where(f => f.Key == key).AsNoTracking().FirstOrDefaultAsync();
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
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
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
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.Preferences.AsNoTracking().ToListAsync();
        }
    }
}
