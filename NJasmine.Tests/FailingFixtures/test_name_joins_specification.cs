using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit]
    //[RunExternal(false, ExpectedStrings = new string[] { @"in progress......"})]
    public class test_name_joins_specification : NJasmineFixture
    {
        public override void Tests()
        {
            it("simple test");
            
            describe("simple describe", delegate
            {
                it("simple test");
            });

            describe("describe with setup", delegate
            {
                //arrange("simple arrange");

                it("simple test");
            });

        }
    }
}
