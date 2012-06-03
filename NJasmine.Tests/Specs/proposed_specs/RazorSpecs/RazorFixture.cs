using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace NJasmineTests.Specs.proposed_specs.RazorSpecs
{
    public abstract class RazorFixture : GivenWhenThenFixture
    {
        public override void Specify()
        {
            throw new NotImplementedException();
        }

        public new string describe(string description, Action action)
        {
            base.describe(description, action);
            return "";
        }

        public new string it(string description, Action action)
        {
            base.it(description, action);
            return "";
        }
    }
}
