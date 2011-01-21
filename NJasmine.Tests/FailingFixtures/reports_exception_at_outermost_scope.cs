using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedStrings = new []{
            "Test Failure : NJasmineTests.FailingFixtures.reports_exception_at_outermost_scope",
            "Attempted to divide by zero."})]
    public class reports_exception_at_outermost_scope : NJasmineFixture
    {
        public override void Specify()
        {
            it("outer test", delegate() { });

            describe("broken describe", delegate()
            {
                it("inner test", delegate()
                {
                });
            });

            it("last test", delegate() { });

            int j = 5;
            int i = 1 / (j - 5);
        }
    }
}