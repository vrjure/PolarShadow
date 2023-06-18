using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Storage;
using PolarShadow.Videos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class WatchRecordService : IWatchRecordService
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;

        public WatchRecordService(IDbContextFactory<PolarShadowDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task AddOrUpdateRecordAsync(WatchRecord record)
        {
            using(var context = _dbFactory.CreateDbContext())
            {
                if (record.Id != 0)
                {
                    context.Record.Update(record);
                }
                else
                {
                    var exist = await context.Record.AsNoTracking().FirstOrDefaultAsync(f => f.Name == record.Name && f.EpisodeName == record.EpisodeName);
                    if (exist != null)
                    {
                        record.Id = exist.Id;
                        context.Record.Update(record);
                    }
                    else
                    {
                        context.Record.Add(record);
                    }
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteRecordAsync(WatchRecord record)
        {
            using (var context = _dbFactory.CreateDbContext())
            {
                context.Record.Remove(record);
                await context.SaveChangesAsync();
            }
        }

        public async Task<ICollection<WatchRecord>> GetRecordsAsync(string videoName)
        {
            using (var context = _dbFactory.CreateDbContext())
            {
                return await context.Record.Where(f => f.Name == videoName).AsNoTracking().ToListAsync();
            }
        }
    }
}
