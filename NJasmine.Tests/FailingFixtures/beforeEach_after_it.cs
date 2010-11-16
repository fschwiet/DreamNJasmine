using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Integration;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedStrings = new[] 
            {"Test Failure : NJasmineTests.FailingFixtures.beforeEach_after_it", 
            "Exception thrown within test definition: Called beforeEach() after disposing()."})]
    public class beforeEach_after_it : NJasmineFixture
    {
        public override void Tests()
        {
            it("hi", delegate{ });

            beforeEach(delegate { });
        }
    }
}
