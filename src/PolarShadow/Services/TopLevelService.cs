using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Services
{
    internal class TopLevelService : ITopLevelService
    {
        private TopLevel _topLevel;
        private Func<Visual> _visualFactory;

        public TopLevel GetTopLevel()
        {
            var topLevel = _topLevel;
            if (topLevel == null)
                topLevel = TopLevel.GetTopLevel(_visualFactory?.Invoke());

            return topLevel;
        }

        public void SetTopLevel(TopLevel topLevel)
        {
            _topLevel = topLevel;
        }

        public void SetTopLevelFactory(Func<Visual> factory)
        {
            _visualFactory = factory;
        }

        public static TopLevel TopLevel => Ioc.Default.GetService<ITopLevelService>()?.GetTopLevel();
    }
}
