using System;
using NJasmineTests.Core;

namespace NJasmineTests.PassingFixtures
{
    [RunExternal(true, ExpectedTraceSequence = @"
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
")]
    public class suite_arranging_disposables : TraceableNJasmineFixture
    {
        public class ObservableDisposable : IDisposable
        {
            public ObservableDisposable()
            {
                TraceableNJasmineFixture.Trace("creating " + TraceableNJasmineFixture.GetTypeShortName(this.GetType()));
            }

            public void Dispose()
            {
                TraceableNJasmineFixture.Trace("disposing " + TraceableNJasmineFixture.GetTypeShortName(this.GetType()));
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
            ResetTracing();

            var a = arrange<some_observable_A>();
            var b = arrange(() => new some_observable_B());

            describe("first describe block", delegate
            {
                var c = arrange(() => new some_observable_C());
                
                it("a test", delegate
                {
                    expect(a).not.to.Be.Null();
                    expect(b).not.to.Be.Null();
                    expect(c).not.to.Be.Null();
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
                            expect(a).not.to.Be.Null();
                            expect(b).not.to.Be.Null();
                            expect(d).not.to.Be.Null();
                        });

                        it("inline using", delegate
                        {
                            var e = arrange<some_observable_E>();
                            var f = arrange(() => new some_observable_F());

                            expect(e).not.to.Be.Null();
                            expect(f).not.to.Be.Null();
                        });
                    });
                });
            });
        }
    }
}
