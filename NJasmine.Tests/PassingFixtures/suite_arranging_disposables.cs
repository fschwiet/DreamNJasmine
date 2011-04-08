using System;
using NJasmineTests.Core;
using Should.Fluent;

namespace NJasmineTests.PassingFixtures
{
    [RunExternal(true, ExpectedTraceSequence = @"
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
")]
    public class suite_arranging_nondisposables : GivenWhenThenFixtureTracingToConsole
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

            var a = arrange<some_observable_A>();
            var b = arrange(() => new some_observable_B());

            describe("first describe block", delegate
            {
                var c = arrange(() => new some_observable_C());
                
                it("a test", delegate
                {
                    i.Should().Equal(123);
                    a.Should().Not.Be.Null();
                    b.Should().Not.Be.Null();
                    c.Should().Not.Be.Null();
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
                            a.Should().Not.Be.Null();
                            b.Should().Not.Be.Null();
                            d.Should().Not.Be.Null();
                        });

                        it("inline using", delegate
                        {
                            var e = arrange<some_observable_E>();
                            var f = arrange(() => new some_observable_F());

                            e.Should().Not.Be.Null();
                            f.Should().Not.Be.Null();
                        });
                    });
                });
            });
        }
    }
}
