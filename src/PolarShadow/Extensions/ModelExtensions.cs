using Avalonia.Controls.Shapes;
using PolarShadow.Core;
using PolarShadow.Models;
using PolarShadow.Resources;
using PolarShadow.Services;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    public static class ModelExtensions
    {

        public static ResourceModel ToResourceModel(this Resource resource)
        {
            return new ResourceModel
            {
                Description = resource.Description,
                FromRequest = resource.FromRequest,
                ImageSrc = resource.ImageSrc,
                Name = resource.Name,
                Request = resource.Request,
                Site = resource.Site,
                Src = resource.Src,
                SrcType = resource.SrcType
            };
        }

    }
}
