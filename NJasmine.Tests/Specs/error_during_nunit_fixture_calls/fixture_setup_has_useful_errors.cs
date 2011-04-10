using System;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs.error_during_nunit_fixture_calls
{
    [Explicit, RunExternal(false, ExpectedStrings = new []
    {
        "NJasmineTests.Specs.error_during_nunit_fixture_calls.fixture_setup_has_useful_errors, when in some context, then there is some text",
        "System.TimeZoneNotFoundException : no time!"
    })]
    public class fixture_setup_has_useful_errors : GivenWhenThenFixture
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
