using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NJasmine;
using NJasmine.Core;
using NJasmine.Core.GlobalSetup;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    public class GlobalSetupManagerTests : GivenWhenThenFixture
    {
        public override void Specify()
        {
            List<string> recording = new List<string>();
            int tracer = 0;

            given("a fixture with global setup", delegate
            {
                var fixture = new LambdaFixture()
                {
                    LambdaSpecify = f =>
                    {
                        var firstSetup = f.beforeAll(delegate
                        {
                            recording.Add("beforeAll " + tracer++);
                            return "beforeAll result " + tracer++;
                        });

                        f.afterAll(delegate
                        {
                            recording.Add("first setup was: " + firstSetup);
                        });

                        f.it("tests something", delegate
                        {
                        });

                        f.describe("some nested tests", delegate
                        {
                            f.beforeAll(() => recording.Add("nested beforeAll " + tracer++));
                            f.afterAll(delegate
                            {
                                recording.Add("nested afterAll " + tracer++);
                            });

                            f.it("a nested test", delegate
                            {
                                int i = 0;
                            });
                        });

                        f.describe("more nested tests", delegate
                        {
                            f.it("has a test", delegate
                            {

                            });
                        });
                    }
                };

                var sut = arrange(() => new GlobalSetupManager(() => fixture));

                when("a test is going to be run", delegate
                {
                    beforeEach(() => sut.PrepareForTestPosition(new TestPosition(2)));

                    then("the global setup has ran", delegate
                    {
                        Assert.That(recording, Is.EquivalentTo(new[] {"beforeAll 0"}));
                    });

                    then("the global setup result is available", delegate
                    {
                        expect(() => "beforeAll result 1" == sut.GetSetupResultAt<string>(new TestPosition(0)));
                    });

                    when("a later nested test is going to be ran", delegate
                    {

                        beforeEach(() => sut.PrepareForTestPosition(new TestPosition(3, 2)));

                        then("the global setup has ran", delegate
                        {
                            Assert.That(recording, Is.EquivalentTo(new[]
                            {
                                "beforeAll 0",
                                "nested beforeAll 2",
                            }));
                        });

                        when("a later test nested in another block is going to be ran", delegate
                        {
                            beforeEach(() => sut.PrepareForTestPosition(new TestPosition(4, 0)));

                            then("the global setup has ran", delegate
                            {
                                Assert.That(recording, Is.EquivalentTo(new[]
                                {
                                    "beforeAll 0",
                                    "nested beforeAll 2",
                                    "nested afterAll 3"
                                }));
                            });
                        });

                        when("we're done running tests", delegate
                        {
                            beforeEach(() => sut.Cleanup(new TestPosition()));

                            then("expected cleanup runs", delegate
                            {
                                Assert.That(recording, Is.EquivalentTo(new[]
                                {
                                    "beforeAll 0",
                                    "nested beforeAll 2",
                                    "nested afterAll 3",
                                    "first setup was: beforeAll result 1"
                                }));
                            });

                            then("the setup thread finishes", delegate
                            {
                                expect(() => !sut.HasThread());
                            });
                        });
                    });
                });
            });

            given("a fixture with an error at its outer scope", delegate
            {
                var fixture = new LambdaFixture()
                {
                    LambdaSpecify = f =>
                    {
                        int i = 0;
                        int j = 100 / i;

                        it("has a test", delegate
                        {

                        });
                    }
                };

                var sut = arrange(() => new GlobalSetupManager(() => fixture));

                cleanup(() => sut.Close());

                when("a test is to be ran", delegate
                {
                    Exception exception = null;

                    beforeEach(() => sut.PrepareForTestPosition(new TestPosition(0)));

                    then("the exception is reported", delegate
                    {
                        expect(() => exception != null);
                    });

                    when("we're done running tests", delegate
                    {
                        expect(() => sut.HasThread());
                        beforeEach(() => sut.Cleanup(new TestPosition()));

                        then("the setup thread finishes", delegate
                        {
                            expect(() => !sut.HasThread());
                        });
                    });
                });
            });
        }
    }
}
