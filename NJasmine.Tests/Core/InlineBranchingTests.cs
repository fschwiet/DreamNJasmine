using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    public class InlineBranchingTests : GivenWhenThenFixture
    {
        public override void Specify()
        {
            describe("RunBranchOption", delegate
            {
                it("runs a branch up to where it joins, which is indicated by an exception", delegate
                {
                    List<string> recording = new List<string>();

                    InlineBranching.RunBranchOption(join =>
                    {
                        recording.Add("one");

                        join();

                        recording.Add("two");
                    });

                    Assert.That(recording, Is.EquivalentTo(new []
                    {
                       "one" 
                    }));
                });
            });
            
            describe("HandleInlineBranches", delegate
            {
                it("runs the action for each branch", delegate
                {
                    var branchPosition = new TestPosition(1, 2, 3);

                    var branchesVisited = 0;
                    List<string> recording = new List<string>();

                    InlineBranching.HandleInlineBranches(branchPosition, new Action<Action>[] { 
                        join => { recording.Add("first branch"); },
                        join => { recording.Add("second branch"); },
                        join => { recording.Add("third branch");},
                    }.ToArray(),
                    (branch, subbranchPosition) =>
                    {
                        InlineBranching.RunBranchOption(branch);

                        branchesVisited++;
                        recording.Add(subbranchPosition.ToString());
                    });

                    expect(() => branchesVisited == 3);
                    Assert.That(recording, Is.EquivalentTo(new []
                    {
                        "first branch",
                        new TestPosition(1,2,3,0).ToString(),
                        "second branch",
                        new TestPosition(1,2,3,1).ToString(),
                        "third branch",
                        new TestPosition(1,2,3,2).ToString(),
                    }));
                });
            });
        }
    }
}
