using NJasmineTests.Core;
using NJasmineTests.Export;

namespace NJasmineTests.Specs.cleanup_any_IDisposable
{
    public class setup_results_dont_have_to_be_disposable : GivenWhenThenFixtureTracingToConsole, INJasmineInternalRequirement
    {
        public class ObservableNondisposable
        {
            public ObservableNondisposable()
            {
                GivenWhenThenFixtureTracingToConsole.Trace("creating " + GivenWhenThenFixtureTracingToConsole.GetTypeShortName(this.GetType()));
            }
        }

        public class some_observable_A : ObservableNondisposable { }
        public class some_observable_B : ObservableNondisposable { }
        public class some_observable_C : ObservableNondisposable { }
        public class some_observable_D : ObservableNondisposable { }
        public class some_observable_E : ObservableNondisposable { }
        public class some_observable_F : ObservableNondisposable { }
        
        public override void Specify()
        {
            beforeAll(ResetTracing);

            var i = arrange(() => 123);

            var a = arrange(() => new some_observable_A());
            var b = arrange(() => new some_observable_B());

            describe("first describe block", delegate
            {
                var c = arrange(() => new some_observable_C());
                
                it("a test", delegate
                {
                    expect(() => i == 123);
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
                            var e = arrange(() => new some_observable_E());
                            var f = arrange(() => new some_observable_F());

                            expect(() => e != null);
                            expect(() => f != null);
                        });
                    });
                });
            });
        }

        public void Verify_NJasmine_implementation(FixtureResult fixtureResult)
        {
            fixtureResult.succeeds();

            fixtureResult.containsTrace(@"
creating some_observable_A
creating some_observable_B
creating some_observable_C
creating some_observable_A
creating some_observable_B
creating some_observable_D
creating some_observable_A
creating some_observable_B
creating some_observable_D
creating some_observable_E
creating some_observable_F
");
        }
    }
}
