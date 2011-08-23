using System;
using System.Linq.Expressions;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.expectations
{
    [Explicit]
    public class can_wait_for_a_condition_to_be_true : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public int WaitsLeft;

        public bool Ready()
        {
            return WaitsLeft-- <= 0;
        }

        public override void Specify()
        {
            describe("waitUntil() or expectEventually() can be used to have the test wait until a condition passes", delegate
            {
                foreach(var kvp in new Tuple<Action<Expression<Func<bool>>, int?, int?>, string>[]
                {
                    new Tuple<Action<Expression<Func<bool>>, int?,int?>, string> ( waitUntil, "waitUntil"),
                    new Tuple<Action<Expression<Func<bool>>, int?,int?>, string> ( expectEventually, "expectEventually")
                })
                {
                    var delayedAssertionImplementation = kvp.Item1;
                    var delayedAssertionName = kvp.Item2;

                    given("a condition that eventually evaluates to true", delegate
                    {
                        it("a normal expect works when no waits are left", delegate
                        {
                            WaitsLeft = 0;
                            expect(() => Ready());
                        });

                        it("a normal expect fails when waits are left", delegate
                        {
                            WaitsLeft = 1;
                            expect(() => Ready());
                        });

                        it(delayedAssertionName + " will try multiple times", delegate
                        {
                            setWaitTimeout(100);
                            setWaitIncrement(5);

                            WaitsLeft = 1;

                            delayedAssertionImplementation(() => Ready(), null, null);

                            delayedAssertionImplementation(() => Ready(), 2000, null);
                        });
                    });

                    describe(delayedAssertionName + " can be called during discovery", delegate
                    {
                        setWaitTimeout(5);
                        setWaitIncrement(3);

                        delayedAssertionImplementation(() => true, null, null);
                        delayedAssertionImplementation(() => false, null, null);

                        it("doesnt prevent discovery", delegate
                        {

                        });
                    });
                }
            });
        }

        public void Verify_NJasmine_implementation(FixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTest("waitUntil() or expectEventually() can be used to have the test wait until a condition passes, given a condition that eventually evaluates to true, a normal expect works when no waits are left")
                .thatSucceeds();

            fixtureResult.hasTest("waitUntil() or expectEventually() can be used to have the test wait until a condition passes, given a condition that eventually evaluates to true, a normal expect fails when waits are left")
                .thatErrors();

            fixtureResult.hasTest("waitUntil() or expectEventually() can be used to have the test wait until a condition passes, given a condition that eventually evaluates to true, waitUntil will try multiple times")
                .thatSucceeds();

            fixtureResult.hasTest("waitUntil() or expectEventually() can be used to have the test wait until a condition passes, given a condition that eventually evaluates to true, expectEventually will try multiple times")
                .thatSucceeds();

            fixtureResult.hasTest("waitUntil() or expectEventually() can be used to have the test wait until a condition passes, waitUntil can be called during discovery, doesnt prevent discovery")
                .thatErrors();

            fixtureResult.hasTest("waitUntil() or expectEventually() can be used to have the test wait until a condition passes, expectEventually can be called during discovery, doesnt prevent discovery")
                .thatErrors();
        }
    }
}
