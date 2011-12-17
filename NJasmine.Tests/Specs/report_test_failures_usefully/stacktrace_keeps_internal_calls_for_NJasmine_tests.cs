using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.report_test_failures_usefully
{
    [Explicit]
    public class stacktrace_keeps_internal_calls_for_NJasmine_tests : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            given("some context", delegate
            {
                when("some action", delegate
                {
                    then("it fails", delegate()
                    {
                        expect(() => 1 + 2 == 4);
                    });
                });
            });
        }

        public void Verify_NJasmine_implementation(FixtureResult fixtureResult)
        {
            fixtureResult.failed();

            var stackTrace = fixtureResult.withStackTraces().Single();
            Assert.That(stackTrace, Is.StringContaining("NJasmine.Core"));

            Assert.That(TestResultUtil.PatternForNJasmineAnonymousMethod.IsMatch(stackTrace));
        }
    }
}
