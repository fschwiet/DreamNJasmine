using NJasmine;
using NJasmineTests.Integration;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedStrings = new []{
            "Test Failure : NJasmineTests.FailingFixtures.ExceptionThrownAtTopLevel", 
            "Exception thrown within test definition: Attempted to divide by zero."})]
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