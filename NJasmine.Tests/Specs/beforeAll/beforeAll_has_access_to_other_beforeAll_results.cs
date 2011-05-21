using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs.beforeAll
{
    [Explicit]
    public class beforeAll_has_access_to_other_beforeAll_results : GivenWhenThenFixture
    {
        public static int IntSource = 0;

        public override void Specify()
        {
            int a = beforeAll(() => ++IntSource);

            describe("in a nested context", delegate
            {
                int b = beforeAll(() => a);

                describe("in a further nested context", delegate
                {
                    int c = beforeAll(() => b);

                    it("has the same values for all", delegate
                    {
                        expect(() => a == b);
                        expect(() => b == c);
                    });
                });
            });
        }
    }
}
