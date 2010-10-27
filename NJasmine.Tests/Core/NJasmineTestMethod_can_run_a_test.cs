using System;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NUnit.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    [TestFixture]
    public class NJasmineTestMethod_can_run_a_test : ExpectationsFixture
    {
        private class AFixture : ObservableNJasmineFixture
        {
            public override void Tests()
            {
                Observe("1");

                beforeEach(delegate()
                {
                    Observe("2");
                });

                describe("first describe", delegate()
                {
                    Observe("4");

                    afterEach(delegate()
                    {
                        Observe("12");
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
                        Observe("7");

                        afterEach(delegate()
                        {
                            Observe("11");
                        });

                        beforeEach(delegate()
                        {
                            Observe("8");
                        });

                        it("the test", delegate()
                        {
                            Observe("10");
                        });

                        Observe("9");
                    });

                    beforeEach(delegate()
                    {
                        Observe("5");
                    });

                    Observe("6");
                });

                afterEach(delegate()
                {
                    Observe("13");
                });

                Observe("3");
            }
        }

        [Test]
        public void can_be_ran()
        {
            AFixture fixture = new AFixture();

            var sut = NJasmineTestMethod.Create(fixture, new TestPosition(1, 3, 2));

            sut.Run();

            expect(fixture.Observations.ToArray()).to.Equal(
                Enumerable.Range(1, 13).Select(i => i.ToString()).ToArray());
        }

    }
}
