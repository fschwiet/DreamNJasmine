using NJasmineTests.Core;
using NUnit.Framework;

namespace NJasmineTests.PassingFixtures
{
    [RunExternal(true, ExpectedStrings = new string[] {
"1) NotRunnable : NJasmineTests.PassingFixtures.supports_unimplemented_specs, an unimplemented test() block",
"2) NotRunnable : NJasmineTests.PassingFixtures.supports_unimplemented_specs, nested too of course, an unimplemented test() block" })]
    public class supports_unimplemented_specs : TraceableNJasmineFixture
    {
        public override void Specify()
        {
            beforeAll(ResetTracing);

            it("an unimplemented test() block");

            describe("nested too of course", delegate
            {
                it("an unimplemented test() block");
            });
        }
    }
}
