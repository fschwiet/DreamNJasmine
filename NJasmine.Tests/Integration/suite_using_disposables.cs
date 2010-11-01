using System;
using NJasmine;
using NJasmineTests.Core;

namespace NJasmineTests.Integration
{
    public class suite_using_disposables : ObservableNJasmineFixture
    {
        public class ObservableDisposable : IDisposable
        {
            public ObservableDisposable()
            {
                ObservableNJasmineFixture.Trace("creating " + ObservableNJasmineFixture.GetTypeShortName(this.GetType()));
            }

            public void Dispose()
            {
                ObservableNJasmineFixture.Trace("disposing" + ObservableNJasmineFixture.GetTypeShortName(this.GetType()));
            }
        }

        public class some_observable_A : ObservableDisposable { }
        public class some_observable_B : ObservableDisposable { }
        public class some_observable_C : ObservableDisposable { }
        public class some_observable_D : ObservableDisposable { }
        public class some_observable_E : ObservableDisposable { }
        public class some_observable_F : ObservableDisposable { }
        
        public override void Tests()
        {
            var a = Using < some_observable_A>();
            var b = Using(() => new some_observable_B());

            describe("first describe block", delegate
            {
                var c = Using(() => new some_observable_C());
                
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
                    var d = Using(() => new some_observable_D());

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
                            var e = Using<some_observable_E>();
                            var f = Using(() => new some_observable_F());

                            expect(e).not.to.Be.Null();
                            expect(f).not.to.Be.Null();
                        });
                    });
                });
            });
        }
    }
}
