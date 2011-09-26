using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NJasmine.Core.GlobalSetup;
using NJasmineTests.Specs.proposed_specs.inline_branching;

namespace NJasmineTests.Core
{
    public class GlobalSetupManager_handles_inline_branching : GivenWhenThenFixture
    {
        public override void Specify()
        {
            it("can prepare for a test after an inline branch", delegate()
            {
                var sut = new GlobalSetupManager();

                sut.Initialize(() => new can_branch_inline());
                cleanup(() => sut.Close());

                Exception e;
                sut.PrepareForTestPosition(new TestPosition(0,0,0,0,0), out e);

                sut.PrepareForTestPosition(new TestPosition(0, 0, 1, 0, 0), out e);
            });
        }
    }
}
    