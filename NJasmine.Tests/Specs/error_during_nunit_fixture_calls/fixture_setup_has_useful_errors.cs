using System;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.error_during_nunit_fixture_calls
{
    [Explicit]
    public class fixture_setup_has_useful_errors : GivenWhenThenFixture, INJasmineInternalRequirement
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

        public void Verify(TestResult testResult)
        {
            testResult.failed();
            testResult.hasTest("NJasmineTests.Specs.error_during_nunit_fixture_calls.fixture_setup_has_useful_errors, when in some context, then there is some text")
                .withMessage("System.TimeZoneNotFoundException : no time!")
                .thatFailsInAnUnspecifiedManner();
        }
    }
}
