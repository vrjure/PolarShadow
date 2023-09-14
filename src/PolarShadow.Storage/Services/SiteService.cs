using Microsoft.EntityFrameworkCore;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class SiteService : ISiteService
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbContextFacory;
        public SiteService(IDbContextFactory<PolarShadowDbContext> dbContextFactory)
        {
            _dbContextFacory = dbContextFactory;
        }

        public async Task DeleteRequestAsync(string requestName)
        {
            using var dbContext = _dbContextFacory.CreateDbContext();
            await dbContext.Requests.Where(f => f.Name == requestName).ExecuteDeleteAsync();
        }

        public async Task DeleteSiteAsync(string site)
        {
            using var dbContext = _dbContextFacory.CreateDbContext();
            using var trans = dbContext.Database.BeginTransaction();
            try
            {
                await dbContext.Sites.Where(f => f.Name == site).ExecuteDeleteAsync();
                await dbContext.Requests.Where(f => f.SiteName == site).ExecuteDeleteAsync();
                await trans.CommitAsync();
            }
            catch(Exception)
            {
                await trans.RollbackAsync();
                throw;
            }
        }

        public async Task<ICollection<RequestModel>> GetRequesetsAsync(string siteName)
        {
            using var dbContext = _dbContextFacory.CreateDbContext();
            return await dbContext.Requests.Where(f=>f.SiteName == siteName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ICollection<SiteInfoModel>> GetSiteInfoAsync()
        {
            using var dbContext = _dbContextFacory.CreateDbContext();
            var query = from s in dbContext.Sites
                        join r in dbContext.Requests on s.Name equals r.SiteName into g
                        select new SiteInfoModel
                        {
                            Site = s,
                            Requests = g.ToList()
                        };
            return await query.AsNoTrackingWithIdentityResolution().ToListAsync();                  
        }

        public ICollection<SiteInfoModel> GetSiteInfo()
        {
            using var dbContext = _dbContextFacory.CreateDbContext();

            var query = from s in dbContext.Sites
                        join r in dbContext.Requests on s.Name equals r.SiteName
                        select new { s, r };
            var result = query.AsNoTrackingWithIdentityResolution().ToList();
            return result.GroupBy(f=>f.s).Select(f => new SiteInfoModel
            {
                Site = f.Key,
                Requests = f.Select(f=>f.r).ToList()
            }).ToList();
        }

        public async Task<ICollection<SiteModel>> GetSitesAsync()
        {
            using var dbContext = _dbContextFacory.CreateDbContext();
            return await dbContext.Sites.AsNoTracking().ToListAsync();
        }

        public ICollection<SiteModel> GetSites()
        {
            using var dbContext = _dbContextFacory.CreateDbContext();
            return dbContext.Sites.AsNoTracking().ToList();
        }

        public async Task SaveAsync(IEnumerable<SiteInfoModel> sites)
        {
            using var dbContext = _dbContextFacory.CreateDbContext();
            using var trans = dbContext.Database.BeginTransaction();
            try
            {
                await dbContext.Sites.ExecuteDeleteAsync();
                await dbContext.Requests.ExecuteDeleteAsync();

                dbContext.Sites.AddRange(sites.Select(f => f.Site));
                foreach (var item in sites)
                {
                    dbContext.Requests.AddRange(item.Requests);
                }

                await dbContext.SaveChangesAsync();

                await trans.CommitAsync();
            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                throw;
            }
        }

        public void Save(IEnumerable<SiteInfoModel> sites)
        {
            using var dbContext = _dbContextFacory.CreateDbContext();
            using var trans = dbContext.Database.BeginTransaction();
            try
            {
                dbContext.Sites.ExecuteDelete();
                dbContext.Requests.ExecuteDelete();

                dbContext.Sites.AddRange(sites.Select(f => f.Site));
                foreach (var item in sites)
                {
                    dbContext.Requests.AddRange(item.Requests);
                }

                dbContext.SaveChanges();

                trans.Commit();
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
            }
        }

        public async Task SaveAsync(SiteInfoModel site)
        {
            using var dbContext = _dbContextFacory.CreateDbContext();
            dbContext.Sites.Add(site.Site);
            dbContext.Requests.AddRange(site.Requests);

            await dbContext.SaveChangesAsync();
        }
    }
}
