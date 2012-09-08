using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace NJasmineTests.Specs.proposed_specs.sharing_context
{
    public abstract class SharedFixtureBase : GivenWhenThenFixture
    {
        public void becomesContext(string name) {}
    }
}
