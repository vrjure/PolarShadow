using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    [PrimaryKey(nameof(SiteName), nameof(Name))]
    public class RequestEntity
    {
        public string Name { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string SiteName { get; set; }
    }
}
