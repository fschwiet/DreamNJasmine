using System;
using System.Linq;
using NJasmine.Marshalled;
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

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTest("repeated outer test").thatErrors();
            fixtureResult.hasTest("repeated outer test`2").thatErrors();
            fixtureResult.hasTest("repeated outer test`3").thatErrors();

            fixtureResult.hasTest("repeated unimplemented outer test").thatIsNotRunnable();
            fixtureResult.hasTest("repeated unimplemented outer test`2").thatIsNotRunnable();
            fixtureResult.hasTest("repeated unimplemented outer test`3").thatIsNotRunnable();

            fixtureResult.hasTest("repeated describe, repeated inner describe").thatFails();
            fixtureResult.hasTest("repeated describe, repeated inner describe`2").thatFails();
            fixtureResult.hasTest("repeated describe, repeated inner describe`3").thatFails();
        }
    }
}
