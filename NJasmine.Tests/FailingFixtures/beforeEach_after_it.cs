using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit]
    public class beforeEach_after_it : NJasmineFixture
    {
        public override void Tests()
        {
            it("hi", delegate{ });

            beforeEach(delegate { });
        }
    }
}
