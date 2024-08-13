using Microsoft.EntityFrameworkCore;
using PolarShadow.Resources;
using PolarShadow.Services;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal class MineResourceService : SyncAbleService<ResourceModel>, IDbMineResourceService
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbContextFactory;

        public MineResourceService(IDbContextFactory<PolarShadowDbContext> dbContextFactory) :base(dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task DeleteRootResourceAsync(int rootId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            using var trans = dbContext.Database.BeginTransaction();
            try
            {
                await dbContext.Resources.Where(f => f.Id == rootId).ExecuteDeleteAsync();
                await dbContext.Resources.Where(f => f.RootId == rootId).ExecuteDeleteAsync();
                await trans.CommitAsync();
            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                throw;
            }
        }

        public async Task<ResourceModel> GetResourceAsync(int id)
        {
            using var dbcontent = _dbContextFactory.CreateDbContext();
            return await dbcontent.Resources.Where(f => f.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<ResourceModel>> GetRootChildrenAsync(int rootId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Resources.Where(f => f.RootId == rootId && f.Id != rootId).AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<ResourceModel>> GetRootChildrenAsync(int rootId, int level)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Resources.Where(f=>f.RootId == rootId && f.Level == level).AsNoTracking().ToListAsync();
        }

        public async Task<int> GetRootChildrenCountAsync(int rootId, int level)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Resources.CountAsync(f=> f.RootId == rootId && f.Level == level);
        }

        public async Task<ResourceModel> GetRootResourceAsync(string resourceName, string site)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Resources.Where(f => f.ParentId == 0 && f.Name == resourceName && f.Site == site).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<ICollection<ResourceModel>> GetRootResourcesAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Resources.Where(f => f.ParentId == 0).ToListAsync();
        }

        public async Task SaveResourceAsync(ResourceTreeNode tree)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            using var trans = dbContext.Database.BeginTransaction();

            try
            {
                await SaveResourceAsync(tree, dbContext);

                await dbContext.SaveChangesAsync();

                await trans.CommitAsync();
            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                throw;
            }
        }

        private async Task SaveResourceAsync(ResourceTreeNode tree, PolarShadowDbContext dbContext)
        {
            await dbContext.Resources.Where(f => f.Id == tree.Id).ExecuteDeleteAsync();
            await dbContext.Resources.Where(f => f.RootId == tree.Id).ExecuteDeleteAsync();

            dbContext.Resources.AddRange(TreeEnumerable.EnumerateDeepFirst(tree, t => t.Children));
            await dbContext.SaveChangesAsync();

            foreach (var item in TreeEnumerable.EnumerateBreadthFirstWithParentChild(tree, t => t.Children))
            {
                if (item.Key == null && item.Value != null) // first
                {
                    item.Value.RootId = item.Value.Id;
                    item.Value.Level = 0;
                }

                if (item.Key != null && item.Value != null)
                {
                    item.Value.ParentId = item.Key.Id;
                    item.Value.RootId = item.Key.RootId;
                    item.Value.Level = item.Key.Level + 1;
                }

                if (item.Value != null)
                {
                    dbContext.Resources.Update(item.Value);
                }
            }
        }

        public override async Task UploadAsync(ICollection<ResourceModel> data)
        {
            if (data == null || data.Count == 0)
            {
                return;
            }
            var rootList = data.Where(f => f.ParentId == 0);

            using var dbContext = _dbContextFactory.CreateDbContext();
            using var trans = dbContext.Database.BeginTransaction();

            try
            {
                var dbResources = await dbContext.Resources.Where(f => f.ParentId == 0).ToListAsync();
                var addList = rootList.ExceptBy(dbResources.Select(f => (f.Name, f.Site)), f => (f.Name, f.Site));
                foreach (var item in addList)
                {
                    var children = data.Where(f => f.RootId == item.Id && f.Id != item.Id);
                    var tree = CreateEnumerator(item, children).BuildTree();
                    tree.Id = 0;
                    foreach (var child in children)
                    {
                        child.Id = 0;
                    }
                    await SaveResourceAsync(tree, dbContext);
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

        private static IEnumerable<ResourceModel> CreateEnumerator(ResourceModel root, IEnumerable<ResourceModel> children)
        {
            yield return root;
            foreach (var item in children)
            {
                yield return item;
            }
        }
    }
}
