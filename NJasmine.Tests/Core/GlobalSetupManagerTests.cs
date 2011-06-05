using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NJasmine;
using NJasmine.Core;
using NUnit.Core;
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
                    }
                };

                var sut = arrange(() => new GlobalSetupManager());

                arrange(() => sut.Initialize(() => fixture));

                when("a test is going to be run", delegate
                {
                    act(() => sut.PrepareForTestPosition(new TestPosition(2)));
                    
                    then("the global setup has ran", delegate
                    {
                        Assert.That(recording, Is.EquivalentTo(new [] {"beforeAll 0"}));
                    });

                    then("the global setup result is available", delegate
                    {
                        expect(() => "beforeAll result 1" == sut.GetSetupResultAt<string>(new TestPosition(0)));
                    });

                    when("a later nested test is going to be ran", delegate {
                        
                        act(() => sut.PrepareForTestPosition(new TestPosition(3, 2)));

                        then("the global setup has ran", delegate
                        {
                            Thread.Sleep(0);
                            
                            //recording.First();
                            Assert.That(recording, Is.EquivalentTo(new[]
                            {
                                "beforeAll 0", 
                                "nested beforeAll 2",
                            }));

                            Assert.That(recording.First(), Is.EqualTo("beforeAll 0"));
                        });
                    });
                    
                    
                });

                
            });
        }

        class LambdaFixture : GivenWhenThenFixture
        {
            public Action<LambdaFixture> LambdaSpecify;

            public override void Specify()
            {
                LambdaSpecify(this);
            }
        }
    }
}
