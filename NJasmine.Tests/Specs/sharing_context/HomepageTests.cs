using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace NJasmineTests.Specs.sharing_context
{
    class HomepageTests : GivenWhenThenFixture2
    {
        public override void Specify()
        {
            var browser = includeContext<BrowserDriverContext>().Browser;

            arrange(() => browser.Goto("http://www.google.com/"));

            it("supports search", delegate
            {
                expect(() => browser.GetText().Contains("search"));
            });
        }
    }
}
