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
    public class Dy2018Site : IPolarShadowSite, IDownloadAble, IGetDetailAble
    {
        public string Name => "DY2018";
        public string Domain => "www.dy2018.com";

        public SrcType DownloadType => SrcType.Magnet;

        public Dy2018Site()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        public async Task<VideoDetail> GetVideoDetailAsync(VideoSummary summary)
        {
            VideoDetail detail = new VideoDetail(summary)
            {
                SiteName = this.Name
            };

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(summary.DetailSrc, Encoding.GetEncoding("gb2312"));
            var node = doc.DocumentNode.SelectSingleNode("//body/div[1]/div/div[3]/div/div[6]/div[2]/ul//div[@id='Zoom']");

            detail.Name = doc.DocumentNode.SelectSingleNode("//body/div[1]/div/div[3]/div/div[6]/div[1]/h1").InnerText;
            detail.ImageSrc = node.SelectSingleNode("img")?.GetAttributeValue("src", "");

            detail.Description = HttpUtility.HtmlDecode(node.InnerText);

            var downloadNodes = node.SelectNodes("//div[@id='downlist']//a");
            if (downloadNodes != null)
            {
                foreach (var aNode in downloadNodes)
                {
                    VideoEpisode ve = new VideoEpisode()
                    {
                        Name = aNode.InnerText,
                        Sources = new VideoSource[]
                        {
                            new VideoSource
                            {
                                Src = aNode.GetAttributeValue("href", ""),
                                SrcType = SrcType.Magnet
                            }
                        }
                    };
                    detail.Episodes.Add(ve);
                }
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
