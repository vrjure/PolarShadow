using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PolarShadow.Core;
using HtmlAgilityPack;
using System.Net.Http;
using System.Web;

namespace PolarShadow.ResourcePack
{
    public class Dy2018Site : IPolarShadowSite, IDownloadAble
    {
        public string Name => "电影天堂";
        public string Domain => "www.dy2018.com";

        public VideoSourceType DownloadType => VideoSourceType.Magnet;

        public Dy2018Site()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        public async Task<VideoDetail> GetVideoDetailAsync(string detailUrl, VideoSummary summary = default)
        {
            VideoDetail detail = new VideoDetail
            {
                Name = this.Name,
                SourceFrom = this.Domain,
                DetailSrc = detailUrl
            };

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(detailUrl, Encoding.GetEncoding("gb2312"));
            var node = doc.DocumentNode.SelectSingleNode("//body/div[1]/div/div[3]/div/div[6]/div[2]/ul//div[@id='Zoom']");
            detail.ImageSrc = node.SelectSingleNode("img")?.GetAttributeValue("src", "");

            detail.Description = HttpUtility.HtmlDecode(node.InnerText);
            detail.Episodes = new List<VideoEpisode>();

            var downloadNodes = node.SelectNodes("//div[@id='downlist']//a");
            if (downloadNodes != null)
            {
                foreach (var aNode in downloadNodes)
                {
                    VideoEpisode ve = new VideoEpisode
                    {
                        Name = aNode.InnerText,
                        Sources = new VideoSource[]
                        {
                            new VideoSource
                            {
                                Src = aNode.GetAttributeValue("href", ""),
                                SrcType = VideoSourceType.Magnet
                            }
                        }
                    };
                    detail.Episodes.Add(ve);
                }
            }

            return detail;

        }
    }
}
