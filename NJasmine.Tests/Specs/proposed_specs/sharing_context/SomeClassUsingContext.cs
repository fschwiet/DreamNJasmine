using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace NJasmineTests.Specs.proposed_specs.sharing_context
{
    class SomeClassUsingContext : GivenWhenThenFixture
    {
        public override void Specify()
        {
            with<SharedContextFixture>(shared =>
            {
                it("can use the context", delegate()
                {
                    shared.WebDriver.Goto("url");
                });
            });
        }
    }
}
