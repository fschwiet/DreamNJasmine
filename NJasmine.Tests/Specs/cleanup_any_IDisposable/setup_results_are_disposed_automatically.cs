using System;
using NJasmine.Marshalled;
using NJasmineTests.Core;
using NJasmineTests.Export;

namespace NJasmineTests.Specs.cleanup_any_IDisposable
{
    public class setup_results_are_disposed_automatically : GivenWhenThenFixtureTracingToConsole, INJasmineInternalRequirement
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
        public class some_observable_G : ObservableDisposable { }
        
        public override void Specify()
        {
            beforeAll(ResetTracing);

            var a = beforeEach(() => new some_observable_A());
            var b = beforeEach(() => new some_observable_B());

            describe("first describe block", delegate
            {
                var c = beforeEach(() => new some_observable_C());
                
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
                    var disposed = beforeAll(() => new some_observable_G());
                    
                    var leaked = beforeAll(() => new some_observable_G());
                    leakDisposable(leaked);

                    var d = beforeEach(() => new some_observable_D());

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
                            var e = beforeEach(() => new some_observable_E());
                            var f = beforeEach(() => new some_observable_F());

                            expect(() => e != null);
                            expect(() => f != null);
                        });
                    });
                });
            });
        }

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.succeeds();

            fixtureResult.hasTrace(@"
creating some_observable_A
creating some_observable_B
creating some_observable_C
disposing some_observable_C
disposing some_observable_B
disposing some_observable_A
creating some_observable_G
creating some_observable_G
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
disposing some_observable_G
");
        }
    }
}
