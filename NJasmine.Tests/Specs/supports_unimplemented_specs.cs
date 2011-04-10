using NJasmineTests.Core;

namespace NJasmineTests.Specs
{
    [RunExternal(true, ExpectedStrings = new string[] {
"1) NotRunnable : NJasmineTests.Specs.supports_unimplemented_specs, an unimplemented test() block",
"2) NotRunnable : NJasmineTests.Specs.supports_unimplemented_specs, nested too of course, an unimplemented test() block" })]
    public class supports_unimplemented_specs : GivenWhenThenFixtureTracingToConsole
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
