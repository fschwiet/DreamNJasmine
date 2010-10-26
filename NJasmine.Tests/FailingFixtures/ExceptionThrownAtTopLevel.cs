using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, Category("FailureExpected")]
    public class ExceptionThrownAtTopLevel : NJasmineFixture
    {
        public override void Tests()
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