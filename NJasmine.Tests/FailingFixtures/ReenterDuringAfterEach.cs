using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, Category("FailureExpected")]
    public class ReenterDuringAfterEach : NJasmineFixture
    {
        public override void Tests()
        {
            afterEach(delegate()
            {
                it("nested it() is not allowed", delegate() { });
            });

            it("has a valid test", delegate()
            {
            });
        }
    }
}
