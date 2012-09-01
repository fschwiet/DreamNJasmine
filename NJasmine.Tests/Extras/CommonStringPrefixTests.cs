using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Extras;

namespace NJasmineTests.Extras
{
    public class CommonStringPrefixTests : GivenWhenThenFixture
    {
        public override void Specify()
        {
            it("recognizes no strings mean no prefix", () =>
            {
                expect(() => "" == CommonStringPrefix.Of(new string[0]));
            });

            it("recognizes a single string is its own prefix", () =>
            {
                expect(() => "abc" == CommonStringPrefix.Of(new[]
                {
                    "abc"
                }));
            });

            it("recognizes when there is no prefix", () =>
            {
                expect(() => "" == CommonStringPrefix.Of(new[]
                {
                    "abc",
                    "def"
                }));
            });

            it("recognizes when there is a prefix", () =>
            {
                expect(() => "cat " == CommonStringPrefix.Of(new[]
                {
                    "cat abc",
                    "cat def"
                }));
            });
        }
    }
}
