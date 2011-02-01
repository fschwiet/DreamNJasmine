using NJasmineTests.Core;
using NUnit.Framework;

namespace NJasmineTests.PassingFixtures
{
    [Explicit, RunExternal(true, ExpectedTraceSequence = @"
First setup
First test
First cleanup
First setup
Second setup
Second test
Second cleanup
First cleanup
")]
    public class arrange_after_test_is_not_included_in_prior_test : TraceableNJasmineFixture
    {
        public override void Specify()
        {
            importNUnit<PerClassTraceResetFixture>();

            arrange(() => Trace("First setup"));
            afterEach(() => Trace("First cleanup"));

            it("runs a test", () => Trace("First test"));

            arrange(() => Trace("Second setup"));
            afterEach(() => Trace("Second cleanup"));

            it("runs another test", () => Trace("Second test"));
        }
    }
}
