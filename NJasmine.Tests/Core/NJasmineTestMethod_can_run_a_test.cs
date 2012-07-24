using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NJasmine.NUnit;
using NJasmine.NUnit.TestElements;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    [TestFixture]
    public class NJasmineTestMethod_can_run_a_test : PowerAssertFixture
    {
        private class AFixture : ObservableNJasmineFixture
        {
            public override void Specify()
            {
                Observe("1");

                beforeEach(delegate()
                {
                    Observe("2");
                });

                describe("first describe", delegate()
                {
                    Observe("3");

                    afterEach(delegate()
                    {
                        Observe("8");
                    });

                    describe("skipped describe", delegate()
                    {
                        Observe("skipped describe");
                    });

                    it("skipped it", delegate()
                    {
                        Observe("skipped it");
                    });

                    describe("second describe", delegate()
                    {
                        Observe("4");

                        afterEach(delegate()
                        {
                            Observe("7");
                        });

                        beforeEach(delegate()
                        {
                            Observe("5");
                        });

                        it("the test", delegate()
                        {
                            Observe("6");
                        });

                        Observe("-2");
                    });

                    beforeEach(delegate()
                    {
                        Observe("-1");
                    });

                    Observe("-3");
                });

                afterEach(delegate()
                {
                    Observe("-1");
                });

                Observe("-4");
            }
        }

        [Test]
        public void can_be_ran()
        {
            AFixture fixture = new AFixture();

            var sut = new NJasmineTestMethod(() => fixture, TestPosition.At(1, 3, 2), new FakeGlobalSetupManager());

            List<string> ignored;
            NJasmineTestMethod.RunTestMethodInner(sut, new TestResultShim(), out ignored);

            expect_observation_matches(fixture.Observations, 1, 2, 3, 4, 5, 6, 7, -2, -3, -4, 8);
        }

        [Test]
        public void duplicated_runs_dont_accidentally_accumulate_afterEach_calls()
        {
            AFixture fixture = new AFixture();

            var sut = new NJasmineTestMethod(() => fixture, TestPosition.At(1, 3, 2), new FakeGlobalSetupManager());

            List<string> ignored;
            NJasmineTestMethod.RunTestMethodInner(sut, new TestResultShim(), out ignored);

            fixture.ResetObservations();

            NJasmineTestMethod.RunTestMethodInner(sut, new TestResultShim(), out ignored);

            expect_observation_matches(fixture.Observations, 1, 2, 3, 4, 5, 6, -2, -3, -4, 7, 8);
        }

        public void expect_observation_matches(IEnumerable<string> recording, params int[] expected)
        {
            Assert.That(recording, Is.EquivalentTo(expected.Select(e => e.ToString())));
        }
    }
}
