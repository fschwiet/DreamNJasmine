using System;
using System.Linq;
using NJasmineTests.Core;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    public class duplicate_test_names_are_fine : GivenWhenThenFixtureTracingToConsole, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            beforeAll(ResetTracing);

            foreach (var i in Enumerable.Range(1, 3))
            {
                it("repeated outer test", delegate
                {
                    var value = "outer it " + i;
                    Trace(value);
                    throw new Exception(value);
                });

                it("repeated unimplemented outer test");

                describe("repeated describe", delegate
                {
                });

                describe("repeated describe", delegate
                {
                });

                describe("repeated describe", delegate
                {
                    it("repeated inner it", delegate
                    {
                        Trace("repeated inner it " + i);
                    });

                    describe("repeated inner describe", delegate
                    {
                        var numerator = 10;
                        var denominator = 0;
                        var result = numerator/denominator;
                    });
                });
            }
        }

        public void Verify(TestResult testResult)
        {
            testResult.failed();

            testResult.hasTest("NJasmineTests.Specs.duplicate_test_names_are_fine, repeated outer test").thatErrors();
            testResult.hasTest("NJasmineTests.Specs.duplicate_test_names_are_fine, repeated outer test`2").thatErrors();
            testResult.hasTest("NJasmineTests.Specs.duplicate_test_names_are_fine, repeated outer test`3").thatErrors();

            testResult.hasTest("NJasmineTests.Specs.duplicate_test_names_are_fine, repeated unimplemented outer test").thatIsNotRunnable();
            testResult.hasTest("NJasmineTests.Specs.duplicate_test_names_are_fine, repeated unimplemented outer test`2").thatIsNotRunnable();
            testResult.hasTest("NJasmineTests.Specs.duplicate_test_names_are_fine, repeated unimplemented outer test`3").thatIsNotRunnable();

            testResult.hasTest("NJasmineTests.Specs.duplicate_test_names_are_fine, repeated describe, repeated inner describe").thatFails();
            testResult.hasTest("NJasmineTests.Specs.duplicate_test_names_are_fine, repeated describe, repeated inner describe`2").thatFails();
            testResult.hasTest("NJasmineTests.Specs.duplicate_test_names_are_fine, repeated describe, repeated inner describe`3").thatFails();
        }
    }
}
