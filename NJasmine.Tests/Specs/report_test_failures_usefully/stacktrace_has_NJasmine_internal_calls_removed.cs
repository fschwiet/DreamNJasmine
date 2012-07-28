using System.Linq;
using NJasmine;
using NJasmine.Core;
using NJasmine.Marshalled;
using NJasmineTests.Export;
using NUnit.Framework;

// namespace should not contain NJasmine.Tests
namespace NamespaceIsntNJasmineTests
{
    [Explicit]
    public class stacktrace_has_NJasmine_internal_calls_removed : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            trace("Using odd namespace so callstack is filtered.");

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

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.failed();

            var stackTrace = fixtureResult.withStackTraces().Single();
            Assert.That(stackTrace, Is.Not.StringContaining("NJasmine.Core"));
            Assert.That(stackTrace, Is.Not.StringContaining("NJasmine.NUnit"));
        }
    }
}
