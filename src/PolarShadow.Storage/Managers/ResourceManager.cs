using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class ResourceManager
    {
        private PolarShadowDbContext _context;
        public ResourceManager(PolarShadowDbContext context)
        {
            _context = context;
        }

        public async Task SaveResourceAsync(ResourceEntity resource, IEnumerable<KeyValuePair<EpisodeEntity, IEnumerable<LinkEntity>>> episodes)
        {
            _context.Resources.Update(resource);
            await _context.SaveChangesAsync();

            if (episodes == null)
            {
                return;
            }

            foreach (var item in episodes)
            {
                item.Key.ResourceId = resource.Id;
                _context.Episodes.Update(item.Key);
            }

            await _context.SaveChangesAsync();

            foreach (var item in episodes)
            {
                foreach (var link in item.Value)
                {
                    link.ResourceId = resource.Id;
                    link.EpisodeId = item.Key.Id;
                    _context.Links.Update(link);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ResourceEntity> GetResourceAsync(int resourceId)
        {
            return await _context.Resources.Where(f=>f.Id == resourceId).FirstOrDefaultAsync();
        }

        public async Task<ResourceEntity> GetResourceAsync(string name)
        {
            return await _context.Resources.Where(f => f.Name == name).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EpisodeEntity>> GetEpisodesAsync(int resourceId)
        {
            return await _context.Episodes.Where(f => f.ResourceId == resourceId).ToListAsync();
        }

        public async Task<IEnumerable<LinkEntity>> GetLinksAsync(int episodeId)
        {
            return await _context.Links.Where(f=>f.EpisodeId == episodeId).ToListAsync();
        }

        public async Task<IEnumerable<LinkEntity>> GetLinksByResource(int resourceId)
        {
            return await _context.Links.Where(f=>f.ResourceId == resourceId).ToListAsync();
        }

        public async Task DeleteResourceAsync(int resourceId)
        {
            await _context.Resources.Where(f => f.Id == resourceId).ExecuteDeleteAsync();
            await _context.Episodes.Where(f => f.ResourceId == resourceId).ExecuteDeleteAsync();
            await _context.Links.Where(f => f.ResourceId == resourceId).ExecuteDeleteAsync();
        }
    }
}
