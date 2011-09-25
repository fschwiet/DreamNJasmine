using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;
using NUnit.Framework;

namespace NJasmineTests.Core.Discovery
{
    public class NJasmineSuiteBuilderTest : GivenWhenThenFixture
    {
        public override void Specify()
        {
            var branchDestiny = new BranchDestiny();
            
            bool haveRecordedTest = false;
            Action<Test> testVisitor = t => haveRecordedTest = true; 

            var sut = new NJasmineTestSuiteBuilder(null, null, branchDestiny, null, testVisitor);

            given("no path is set for the current branch", delegate
            {
                when("the test discovery forks", delegate
                {
                    var position = new TestPosition(1,2,3);
                    TestPosition continuingAt;

                    arrange(() =>
                    {
                        sut.visitEither(SpecElement.fork, new Action<Action>[]
                        {
                           join => { throw new Exception(); join();},
                           join => { throw new Exception(); join();},
                           join => { throw new Exception(); join();},
                        }, position, out continuingAt);
                    });

                    then("available paths are queued", delegate
                    {
                        expect(() => branchDestiny.GetDiscoveriesQueuedCount() == 3);
                    });

                    //then("discovery stops creating tests for this iteration", delegate
                    //{
                    //    Assert.That(sut, Is.Not.Null);

                    //    sut.visitTest(SpecElement.it, "testing", () => {}, new TestPosition(1,2,3,4));

                    //    expect(() => haveRecordedTest == false);
                    //});
                });
            });


            given("a path is set for the current branch", delegate
            {
                branchDestiny.SetPredeterminedPath(new TestPosition(1, 2, 3, 4));

                when("the test discovery forks", delegate
                {
                    var position = new TestPosition(1, 2);
                    TestPosition continuingAt;

                    bool wasRun = false;

                    Action<Action> expectedBranch = join => { wasRun = true; join(); };

                    sut.visitEither(SpecElement.fork, new Action<Action>[]
                        {
                           join => { throw new Exception(); join();},
                           join => { throw new Exception(); join();},
                           join => { throw new Exception(); join();},
                           expectedBranch,
                           join => { throw new Exception(); join();},
                           join => { throw new Exception(); join();},
                        }, 
                        position, out continuingAt);

                    then("no paths are queued", delegate
                    {
                        expect(() => branchDestiny.GetDiscoveriesQueuedCount() == 0);
                    });

                    then("the targetted branch is ran", delegate
                    {
                        expect(() => wasRun);
                    });

                    then("discovery continues along that branch", delegate
                    {
                        Assert.That(continuingAt, Is.EqualTo(new TestPosition(1,2,3,0)));
                    });
                });
            });
        }
    }
}
