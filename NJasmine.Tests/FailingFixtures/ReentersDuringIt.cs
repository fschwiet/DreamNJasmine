using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, Category("FailureExpected")]
    public class ReentersDuringIt : NJasmineFixture
    {
        public override void Tests()
        {
            it("outer test", delegate()
            {
                it("inner test", delegate() { });
            });
        }
    }
}
