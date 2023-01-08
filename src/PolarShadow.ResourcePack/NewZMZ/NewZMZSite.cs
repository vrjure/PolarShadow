using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PolarShadow.Core;
using HtmlAgilityPack;
using System.Web;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace PolarShadow.ResourcePack
{
    public class NewZMZSite : IPolarShadowSite, ISearchAble, IDownloadAble, IGetDetailAble
    {
        public string Name => "NEWZMZ";

        public string Domain => "newzmz.com";

        public SrcType DownloadType => SrcType.Magnet | SrcType.BaiDu | SrcType.Quark | SrcType.ALiYun;

        public async Task<VideoDetail> GetVideoDetailAsync(string detailUrl, VideoSummary summary = default)
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(detailUrl);
            var desc = doc.DocumentNode.SelectSingleNode("//div[@class='details']//div[@class='aliasname']")?.InnerText;
            var href = doc.DocumentNode.SelectSingleNode("//div[@class='details']/div[@class='details-wrap']/div[@class='epmetas']/a")?.GetAttributeValue("href", "");

            var result = new VideoDetail(summary)
            {
                Name = summary?.Name,
                Description = desc,
                DetailSrc = detailUrl,
                ImageSrc = summary?.ImageSrc,
                SiteName = this.Name
            };
            if (!string.IsNullOrEmpty(href))
            {
                var epDoc = await web.LoadFromWebAsync(href);
                var articleNode = epDoc.DocumentNode.SelectSingleNode("//body/section[2]//article");
                if (articleNode != null)
                {
                    var faq_wrappers = articleNode.SelectNodes("div");
                    foreach (var faq_wrapper in faq_wrappers)
                    {
                        var faq_items = faq_wrapper.SelectSingleNode("div")?.SelectNodes("div");
                        if (faq_items == null)
                        {
                            continue;
                        }

                        foreach (var faq_item in faq_items)
                        {
                            VideoEpisode ve = new VideoEpisode();
                            result.Episodes.Add(ve);

                            var team_con_area = faq_item.SelectSingleNode("div/div");
                            var item_content = team_con_area.SelectSingleNode("div/div[@class='item-content']");

                            ve.Name = item_content.SelectSingleNode("span")?.InnerText;
                            ve.Description = string.Join(" ", item_content.SelectSingleNode("div")?.SelectNodes("a")?.Select(f => f.InnerText));

                            var downloadLis = team_con_area.SelectSingleNode("ul")?.SelectNodes("li");
                            if (downloadLis == null)
                            {
                                continue;
                            }

                            ve.Sources = new List<VideoSource>();
                            foreach (var li in downloadLis)
                            {
                                var a = li.SelectSingleNode("a");

                                var source = new VideoSource();
                                source.Src = a?.GetAttributeValue("href", "");
                                if (!string.IsNullOrEmpty(source.Src))
                                {
                                    source.SrcType = source.Src.GetVideoSourceType();
                                }

                                ve.Sources.Add(source);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public async Task<PageResult<VideoSummary>> SearchVideosAsync(SearchVideoFilter filter, CancellationToken cancellation = default)
        {
            using (var client = new HttpClient())
            {
                var serachUrl = $"https://{Domain}/subres/index/getres.html?keyword={filter.SearchKey}&page={filter.Page}&snpage={filter.PageSize}";
                var result = await client.GetFromJsonAsync<ZmzSearchResult>(serachUrl, JsonOption.DefaultSerializer, cancellation);
                if (result == null)
                {
                    return new PageResult<VideoSummary>
                    {
                        Page = filter.Page,
                        PageSize = filter.PageSize,
                        Total = 0
                    };
                }

                var page = new PageResult<VideoSummary>()
                {
                    Page = result.Page.CurPage == 0 ? 1 : result.Page.CurPage,
                    PageSize = result.Page.PageNum,
                    Total = result.Page.TotalNum
                };

                if (result.Data != null)
                {
                    page.Data = new List<VideoSummary>();
                    foreach (var res in result.Data)
                    {
                        VideoSummary summary = new VideoSummary
                        {
                            DetailSrc = $"https://{Domain}/{res.Link_url}",
                            ImageSrc = res.Image_url,
                            Name = res.Name,
                            SiteName = this.Name
                        };

                        page.Data.Add(summary);
                    }
                }

                return page;
            }
        }
    }
}
