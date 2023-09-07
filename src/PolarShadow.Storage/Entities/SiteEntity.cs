using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class SiteEntity
    {
        [Key]
        public string Name { get; set; }
        public string Domain { get; set; }
    }
}
