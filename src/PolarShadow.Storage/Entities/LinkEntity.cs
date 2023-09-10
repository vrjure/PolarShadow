using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class LinkEntity : Link
    {
        [Key]
        public int Id { get; set; }
        public int EpisodeId { get; set; }
        public int ResourceId { get; set; }
    }
}
