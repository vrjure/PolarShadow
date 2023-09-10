﻿using System;
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
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Domain { get; set; }
    }
}
