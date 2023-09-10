using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class EpisodeEntity
    {
        [Key]
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
    }
}
