using System;
using NJasmineTests.Core;
using NJasmineTests.Export;

namespace NJasmineTests.Specs
{
    public class suite_arranging_disposables : GivenWhenThenFixtureTracingToConsole, INJasmineInternalRequirement
    {
        public class ObservableDisposable : IDisposable
        {
            public ObservableDisposable()
            {
                GivenWhenThenFixtureTracingToConsole.Trace("creating " + GivenWhenThenFixtureTracingToConsole.GetTypeShortName(this.GetType()));
            }

            public void Dispose()
            {
                GivenWhenThenFixtureTracingToConsole.Trace("disposing " + GivenWhenThenFixtureTracingToConsole.GetTypeShortName(this.GetType()));
            }
        }

        public class some_observable_A : ObservableDisposable { }
        public class some_observable_B : ObservableDisposable { }
        public class some_observable_C : ObservableDisposable { }
        public class some_observable_D : ObservableDisposable { }
        public class some_observable_E : ObservableDisposable { }
        public class some_observable_F : ObservableDisposable { }
        
        public override void Specify()
        {
            beforeAll(ResetTracing);

            var a = arrange<some_observable_A>();
            var b = arrange(() => new some_observable_B());

            describe("first describe block", delegate
            {
                var c = arrange(() => new some_observable_C());
                
                it("a test", delegate
                {
                    expect(() => a != null);
                    expect(() => b != null);
                    expect(() => c != null);
                });
            });

            describe("second describe block", delegate
            {
                describe("nested describe block", delegate
                {
                    var d = arrange(() => new some_observable_D());

                    describe("another describe block", delegate
                    {
                        it("a test", delegate
                        {
                            expect(() => a != null);
                            expect(() => b != null);
                            expect(() => d != null);
                        });

                        it("inline using", delegate
                        {
                            var e = arrange<some_observable_E>();
                            var f = arrange(() => new some_observable_F());

                            expect(() => e != null);
                            expect(() => f != null);
                        });
                    });
                });
            });
        }

        public void Verify(FixtureResult fixtureResult)
        {
            fixtureResult.succeeds();

            fixtureResult.containsTrace(@"
creating some_observable_A
creating some_observable_B
creating some_observable_C
disposing some_observable_C
disposing some_observable_B
disposing some_observable_A
creating some_observable_A
creating some_observable_B
creating some_observable_D
disposing some_observable_D
disposing some_observable_B
disposing some_observable_A
creating some_observable_A
creating some_observable_B
creating some_observable_D
creating some_observable_E
creating some_observable_F
disposing some_observable_F
disposing some_observable_E
disposing some_observable_D
disposing some_observable_B
disposing some_observable_A
");
        }
    }
}
