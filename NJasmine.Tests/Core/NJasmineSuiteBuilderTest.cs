using System;
using NJasmine;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    public class NJasmineSuiteBuilderTest : GivenWhenThenFixture
    {
        public override void Specify()
        {
            var branchDestiny = new BranchDestiny();
            
            var sut = new NJasmineTestSuiteBuilder(null, null, branchDestiny, null, null);

            given("no path is set for the current branch", delegate
            {
                when("the test discovery forks", delegate
                {
                    var position = new TestPosition(1,2,3);
                    TestPosition updatedPosition = null;

                    bool wasExpectedBranchRan = false;
                    Action<Action> expectedBranch = join => { wasExpectedBranchRan = true; join(); };

                    arrange(() =>
                    {
                        sut.visitEither(SpecElement.fork, new Action<Action>[]
                        {
                           join => { wasExpectedBranchRan = true; join(); },
                           join => { throw new Exception(); join();},
                           join => { throw new Exception(); join();},
                        }, position, p => updatedPosition = p);
                    });

                    then("the first branch runs", delegate
                    {
                        expect(() => wasExpectedBranchRan);
                    });

                    then("continuing at the first subpath", delegate
                    {
                        Assert.That(updatedPosition, Is.EqualTo(new TestPosition(1, 2, 3, 0, 0)));
                    });

                    then("remaining paths are queued", delegate
                    {
                        expect(() => branchDestiny.GetDiscoveriesQueuedCount() == 2);
                    });
                });
            });


            given("a path is set for the current branch", delegate
            {
                branchDestiny.SetPredeterminedPath(new TestPosition(1, 2, 3, 4));

                when("the test discovery forks", delegate
                {
                    var position = new TestPosition(1, 2);
                    TestPosition updatedPosition = null;

                    bool wasExpectedBranchRan = false;
                    Action<Action> expectedBranch = join => { wasExpectedBranchRan = true; join(); };

                    sut.visitEither(SpecElement.fork, new Action<Action>[]
                        {
                           join => { throw new Exception(); join();},
                           join => { throw new Exception(); join();},
                           join => { throw new Exception(); join();},
                           expectedBranch,
                           join => { throw new Exception(); join();},
                           join => { throw new Exception(); join();},
                        },
                        position, p => updatedPosition = p);

                    then("no paths are queued", delegate
                    {
                        expect(() => branchDestiny.GetDiscoveriesQueuedCount() == 0);
                    });

                    then("the targetted branch is ran", delegate
                    {
                        expect(() => wasExpectedBranchRan);
                    });

                    then("discovery continues along that branch", delegate
                    {
                        Assert.That(updatedPosition, Is.EqualTo(new TestPosition(1,2,3,0)));
                    });
                });
            });
        }
    }
}
