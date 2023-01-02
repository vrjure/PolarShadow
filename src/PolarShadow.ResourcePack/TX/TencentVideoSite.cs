using HtmlAgilityPack;
using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PolarShadow.ResourcePack
{
    public class TencentVideoSite
    {
        public string Name => "腾讯视频";

        public string Domain => "v.qq.com";

        public Task<VideoDetail> GetVideoDetailAsync(string detailSrc, VideoSummary summary = null)
        {
            throw new NotImplementedException();
        }

        public async Task<PageResult<VideoSummary>> SearchVideosAsync(SearchVideoFilter filter, CancellationToken cancellation = default)
        {
            PageResult<VideoSummary> page = new PageResult<VideoSummary>();
            page.Page = filter.Page;
            page.PageSize = filter.PageSize;

            var searchUrl = $"https://v.qq.com/x/search/?q={HttpUtility.UrlEncode(filter.SearchKey)}";
            var webClient = new HtmlWeb();
            var doc = await webClient.LoadFromWebAsync(searchUrl, cancellation);
            if (doc == null)
            {
                return page;
            }

            var wrapperMain = doc.DocumentNode.SelectSingleNode("//body//div[@class='search_container']/div[@class='wrapper']/div[@class='wrapper_main']");
            if (wrapperMain != null)
            {
                return page;
            }

            var series = wrapperMain.SelectSingleNode("//div[@class='result_series result_intention']");
            if (series != null)
            {
                var figureList = series.SelectSingleNode("//ul[@class='figures_list _cont']")?.SelectNodes("li");
                if (figureList == null)
                {
                    return page;
                }

                page.Data = new List<VideoSummary>();
                foreach (var li in figureList)
                {
                    var name = li.SelectSingleNode("//strong//a")?.GetAttributeValue("title", "");
                    var a = li.SelectSingleNode("//strong//a")?.GetAttributeValue("href", "");
                    var desc = li.SelectSingleNode("//div[@class='figure_desc']")?.InnerText;
                    var img = li.SelectSingleNode("//img[@class='figure_pic']")?.GetAttributeValue("src", "");
                    page.Data.Add(new VideoSummary
                    {
                        Name = name,
                        Description = desc,
                        SiteName = Name,
                        DetailSrc = a,
                        ImageSrc = img
                    });
                }
                return page;
            }

            var mixWrapper = wrapperMain.SelectSingleNode("//div[@class='mix_warp']");
            if (mixWrapper == null)
            {
                return page;
            }

            var list = mixWrapper.SelectNodes("div");
            if (list == null)
            {
                return page;
            }

            page.Data = new List<VideoSummary>();
            foreach (var div in list)
            {
                var info = div.SelectSingleNode("//div[@class='_infos']");
                var detailSrc = info.SelectSingleNode("div/h2/a")?.GetAttributeValue("href", "");
                var name = info.SelectSingleNode("div/h2/a/em")?.InnerText;
                var img = info.SelectSingleNode("div/a/img")?.GetAttributeValue("src", "");
                var desc = info.SelectSingleNode("//span[@class='desc_text']").InnerText;
                page.Data.Add(new VideoSummary
                {
                    Name = name,
                    Description = desc,
                    DetailSrc = detailSrc,
                    ImageSrc = img,
                    SiteName = Name
                });
            }

            return page;
        }
    }
}
