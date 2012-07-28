using System.Text.RegularExpressions;
using NJasmine;
using NJasmine.Extras;
using NJasmine.Marshalled;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.report_test_failures_usefully
{
    [Explicit]
    public class stacktrace_shows_fileposition_first : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.hasTest("shows stacktrace information for this failing test")
                .withDetailedMessageThat(message =>
                {
                    Expect.That(() => Regex.Match(message, "at stacktrace_shows_fileposition_first.cs:\\d+ ").Success);
                });
        }

        public override void Specify()
        {

            it("shows stacktrace information for this failing test", delegate
            {
                expect(() => false);
            });
        }
    }
}
