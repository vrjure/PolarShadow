using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace PolarShadowTests
{
    public class AnalysisTest
    {
        [Test]
        public async Task AnalysisAbilityHtmlTest()
        {
            var ability = new AnalysisAbility()
            {
                Url = $"https://www.yhdmp.cc/s_all?kw={HttpUtility.UrlEncode("死神")}&pagesize=10&pageindex=0",
                AnalysisType = AnalysisType.Html,
                Analysis = new Dictionary<string, AnalysisAction>
                {
                    {
                        "data",
                        new AnalysisAction
                        {
                            Path = "//body//div[@class='lpic']/ul/li",
                            PathValueType = PathValueType.Array,
                            AnalysisItem = new Dictionary<string, AnalysisAction>
                            {
                                {
                                    "name",
                                    new AnalysisAction
                                    {
                                        Path = "h2/a",
                                        PathValueType = PathValueType.InnerText
                                    }
                                },
                                {
                                    "description",
                                    new AnalysisAction
                                    {
                                        Path = "p",
                                        PathValueType = PathValueType.InnerText
                                    }
                                },
                                {
                                    "siteName",
                                    new AnalysisAction
                                    {
                                        Path = "YHDM",
                                        PathValueType = PathValueType.None
                                    }
                                },
                                {
                                    "imageSrc",
                                    new AnalysisAction
                                    {
                                        Path = "a/img",
                                        PathValueType = PathValueType.Attribute,
                                        AttributeName = "href"
                                    }
                                }
                            }
                        }
                    },
                    {
                        "total",
                        new AnalysisAction
                        {
                            Path = "//div[@class='gohome']/h1",
                            PathValueType = PathValueType.InnerText,
                            Regex = "\\d+"
                        }
                    }
                }
            };

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(ability.Url);
            var page = doc.DocumentNode.Analysis<PageResult<VideoSummary>>(ability.Analysis);
            Console.WriteLine(JsonSerializer.Serialize(page, JsonOption.DefaultSerializer));
        }
    }
}
