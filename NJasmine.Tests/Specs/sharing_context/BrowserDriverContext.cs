using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace NJasmineTests.Specs.sharing_context
{

    public class BrowserDriverContext : GivenWhenThenContext
    {
        public IWebDriver Browser;

        public override void Specify()
        {
            given("the browser is firefox", delegate
            {
                Browser = arrange(() => new FirefoxBrowser());

                go();
            });

            // maybe support skip remaining tests if any have failed so far?

            given("the browser is internet explorer", delegate
            {
                Browser = arrange(() => new InternetExplorerBroser());

                go();
            });

            given("the browser is internet explorer", delegate
            {
                Browser = arrange(() => new ChromeBrowser());

                go();
            });
        }
    }
}
