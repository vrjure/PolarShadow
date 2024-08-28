using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    public interface IMineResourceService: ISyncAble<ResourceModel>
    {
        /// <summary>
        /// 相同名的的根节点资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        Task<ResourceModel> GetRootResourceAsync(string resourceName, string site);
        /// <summary>
        /// 所有的根节点资源
        /// </summary>
        /// <returns></returns>
        Task<ICollection<ResourceModel>> GetRootResourcesAsync();
        /// <summary>
        /// 根节点下的所有资源
        /// </summary>
        /// <param name="rootId"></param>
        /// <returns></returns>
        Task<ICollection<ResourceModel>> GetRootChildrenAsync(long rootId);
        /// <summary>
        /// 获取根节点下level级的资源
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        Task<ICollection<ResourceModel>> GetRootChildrenAsync(long rootId, int level);
        /// <summary>
        /// 获取根节点下level级的资源数
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        Task<int> GetRootChildrenCountAsync(long rootId, int level);
        /// <summary>
        /// 获取单个资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResourceModel> GetResourceAsync(long id);
        /// <summary>
        /// 保存资源树
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        Task SaveResourceAsync(ResourceTreeNode tree);
        /// <summary>
        /// 删除根节点资源及其下所有资源
        /// </summary>
        /// <param name="rootId"></param>
        /// <returns></returns>
        Task DeleteRootResourceAsync(long rootId);
    }
}
