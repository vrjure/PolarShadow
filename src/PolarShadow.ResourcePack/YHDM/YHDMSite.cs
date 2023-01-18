using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Web;
using System.Text.RegularExpressions;
using System.Linq;

namespace PolarShadow.ResourcePack
{
    public class YHDMSite : IPolarShadowSite, ISearchAble, IGetDetailAble
    {
        public string Name => "YHDM";

        public string Domain => "www.yhdmp.cc";

        public async Task<PageResult<VideoSummary>> SearchVideosAsync(SearchVideoFilter filter, CancellationToken cancellation = default)
        {
            PageResult<VideoSummary> page = new PageResult<VideoSummary>();
            page.Data = new List<VideoSummary>(filter.PageSize);
            page.Page = filter.Page;

            var url = $"https://www.yhdmp.cc/s_all?kw={HttpUtility.UrlEncode(filter.SearchKey)}&pagesize={filter.PageSize}&pageindex={filter.Page - 1}";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var totalText = doc.DocumentNode.SelectSingleNode("//div[@class='gohome']/h1")?.InnerText;

            var ul = doc.DocumentNode.SelectSingleNode("//body//div[@class='lpic']/ul");
            if (ul == null)
            {
                return page;
            }


            if (!string.IsNullOrEmpty(totalText))
            {
                var match = Regex.Match(totalText, "\\d+");
                if (match.Success)
                {
                    page.Total = Convert.ToInt32(match.Value);
                }
            }

            var liList = ul.SelectNodes("li");
            if (liList == null)
            {
                return page;
            }
            foreach (var li in liList)
            {
                var summary = new VideoSummary();
                summary.SiteName = this.Name;
                summary.DetailSrc = $"https://{Domain}{li.SelectSingleNode("a").GetAttributeValue("href", "")}";
                summary.ImageSrc = "https:" + li.SelectSingleNode("a/img").GetAttributeValue("src", "");
                summary.Name = li.SelectSingleNode("h2/a").InnerText;
                summary.Description = li.SelectSingleNode("p").InnerText;

                page.Data.Add(summary);
            }


            return page;
        }

        public async Task<VideoDetail> GetVideoDetailAsync(string detailSrc, VideoSummary summary = null)
        {
            var detail = new VideoDetail(summary);

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(detailSrc);

            var tabs = doc.DocumentNode.SelectSingleNode("//body//div[@class='tabs']");
            var defaultIndexStr = tabs.SelectSingleNode("script[@id='DEF_PLAYINDEX']")?.InnerText;
            var defaultIndex = 0;
            if (!string.IsNullOrEmpty(defaultIndexStr))
            {
                defaultIndex = Convert.ToInt32(defaultIndexStr);
            }

            var main = tabs.SelectSingleNode("div[@id='main0']");
            var playList = main.SelectNodes("div");

            var select = playList.Skip(defaultIndex).FirstOrDefault();
            if (select == null)
            {
                return detail;
            }
            
            var ul = select.SelectSingleNode("ul");
            if (ul == null)
            {
                return detail;
            }
            var liList = ul.SelectNodes("li");
            foreach (var li in liList)
            {
                var a = li.SelectSingleNode("a");
                var ve = new VideoEpisode()
                {
                    Name = a.InnerText,
                    Description = string.Empty,
                    Sources = new List<VideoSource>()
                    {
                        new VideoSource{Src = $"https://{Domain}{a.GetAttributeValue("href", "")}", SrcType = SrcType.HTML}
                    }
                };

                detail.Episodes.Add(ve);
            }

            return detail;

        }

        public bool HasAbility(string abilityName)
        {
            throw new NotImplementedException();
        }

        public object GetAbility(string abilityName)
        {
            throw new NotImplementedException();
        }
    }
}
