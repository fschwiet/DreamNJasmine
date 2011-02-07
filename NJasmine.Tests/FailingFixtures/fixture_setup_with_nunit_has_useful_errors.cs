using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedStrings = new []
    {
        "NJasmineTests.FailingFixtures.fixture_setup_with_nunit_has_useful_errors_on_weapons, when in some context, then there is some text",
        "System.TimeZoneNotFoundException : no time!"
    })]
    public class fixture_setup_with_nunit_has_useful_errors : GivenWhenThenFixture
    {
        public override void Specify()
        {
            when("in some context", delegate
            {
                importNUnit<SomeFixture>();

                then("there is some text", delegate
                {
                    
                });
            });

            importNUnit<SomeFixture>();
        }

        public class SomeFixture
        {
            [TestFixtureSetUp]
            public void FixtureSetup()
            {
                throw new TimeZoneNotFoundException("no time!");
            }
        }
    }
}
