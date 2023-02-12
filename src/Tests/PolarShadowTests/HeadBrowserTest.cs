using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadowTests
{
    public class HeadBrowserTest
    {
        [Test]
        public async Task PuppeteerTest()
        {
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false
            });
            var page = await browser.NewPageAsync();
            await page.SetViewportAsync(new ViewPortOptions { Width = 1920, Height = 1080 });
            await page.GoToAsync("https://www.yhdmp.cc");
            await page.WaitForSelectorAsync(".dtit");
            Console.WriteLine(await page.GetContentAsync());
        }
    }
}
