using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadowTests
{
    public class MatchTest
    {

        [Test]
        public void Test()
        {
            var str = "abc_acb_bac_bca_cab_cba";
            var matchstr = new Dictionary<string, bool>
            {
                { "abc", false },
                { "*abc", false },
                { "abc*", true },
                { "*cba", true },
                { "*bac*", true },
                { "*bcc*", false },
                { "*bac*cab*", true }
            };

            foreach (var item in matchstr)
            {
                Assert.That(RequestRule.MatchWithWildcard(str, item.Key), Is.EqualTo(item.Value));
            }
        }
    }
}
