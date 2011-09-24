using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;

namespace NJasmineTests.Core.Discovery
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

                    sut.visitEither(SpecElement.fork, new Action<Action>[]
                    {
                       join => { throw new Exception(); join();},
                       join => { throw new Exception(); join();},
                       join => { throw new Exception(); join();},
                    }, position);

                    then("available paths are queued", delegate
                    {
                        expect(() => branchDestiny.GetDiscoveriesQueuedCount() == 3);
                    });
                });
            });


            given("a path is set for the current branch", delegate
            {
                branchDestiny._destinedPath = new TestPosition(1, 2, 3, 4);

                when("the test discovery forks", delegate
                {
                    var position = new TestPosition(1, 2);

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
                        position);

                    then("no paths are queued", delegate
                    {
                        expect(() => branchDestiny.GetDiscoveriesQueuedCount() == 0);
                    });

                    then("the targetted branch is ran", delegate
                    {
                        expect(() => wasRun);
                    });
                });
            });
        }
    }
}
