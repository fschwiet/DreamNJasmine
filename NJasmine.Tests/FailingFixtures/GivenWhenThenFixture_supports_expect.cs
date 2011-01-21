using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedStrings = new[] { "IsTrue failed, expression was:" })]
    public class GivenWhenThenFixture_supports_expect : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("nothing in particular", delegate
            {
                then("we want to fail", delegate
                {
                    string a = "Hello, World!";
                    expect(() => a == "Foobar");
                });
            });
        }
    }
}
