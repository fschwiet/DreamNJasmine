using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core
{
    public partial class NJasmineTestMethod
    {
        public class CleanupState : ArrangeState
        {
            public CleanupState(NJasmineTestMethod subject, SpecMethod specMethod)
                : base(subject, specMethod)
            {
            }

            public override void visitAfterEach(Action action, TestPosition position)
            {
                throw DontException(SpecMethod.afterEach);
            }

            public override TArranged visitBeforeEach<TArranged>(SpecMethod origin, string description, IEnumerable<Func<TArranged>> factories, TestPosition position)
            {
                throw DontException(origin);
            }
        }
    }
}
