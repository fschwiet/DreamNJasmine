using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core
{
    public partial class NJasmineTestMethod
    {
        public class ActState : ArrangeState
        {
            public ActState(NJasmineTestMethod subject, SpecElement specElement)
                : base(subject, specElement)
            {
            }
        }
    }
}
