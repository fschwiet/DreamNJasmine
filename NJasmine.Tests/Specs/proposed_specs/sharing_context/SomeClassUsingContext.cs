using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace NJasmineTests.Specs.proposed_specs.sharing_context
{
    class SomeClassUsingContext : SharingFixtureBase
    {
        public override void Specify()
        {
            var shared = with<SharedContextFixture>("with browser");

            it("can use the context", delegate()
            {
                shared.WebDriver.Goto("url");
            });
        }
    }
}
