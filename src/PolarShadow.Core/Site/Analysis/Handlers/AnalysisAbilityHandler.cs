using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal abstract class AnalysisAbilityHandler
    {
        protected readonly AnalysisAbility AbilityConfig;
        private static HtmlWeb _web;
        public AnalysisAbilityHandler(AnalysisAbility ability)
        {
            AbilityConfig = ability;
        }

        protected virtual async Task<T> HandleValueAsync<T>(string url)
        {
            if (AbilityConfig.Analysis == null || AbilityConfig.Analysis.Count == 0)
            {
                return default;
            }
            var formatUrl = AbilityConfig.Url.FormatUrl();
            switch (AbilityConfig.AnalysisType)
            {
                case AnalysisType.Json:
                    break;
                case AnalysisType.Html:
                    if (_web == null)
                    {
                        _web = new HtmlWeb();
                    }

                    var doc = await _web.LoadFromWebAsync(formatUrl);

                    return doc.DocumentNode.Analysis<T>(AbilityConfig.Analysis);
                default:
                    break;
            }

            return default;
        }
    }
}
