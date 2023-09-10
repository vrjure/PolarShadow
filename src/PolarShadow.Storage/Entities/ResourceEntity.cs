using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class ResourceEntity : Link
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 资源描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        public string ImageSrc { get; set; }
    }
}
