using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core
{
    public partial class NJasmineTestMethod
    {
        public class ArrangeState : DescribeState
        {
            protected readonly SpecElement SpecElement;

            public ArrangeState(NJasmineTestMethod subject, SpecElement specElement)
                : base(subject)
            {
                SpecElement = specElement;
            }

            public override void visitFork(SpecElement origin, string description, Action action, TestPosition position)
            {
                throw DontException(origin);
            }

            public override TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
            {
                throw DontException(origin);
            }

            public override void visitAfterAll(SpecElement origin, Action action, TestPosition position)
            {
                throw DontException(origin);
            }

            public override void visitTest(SpecElement origin, string description, Action action, TestPosition position)
            {
                throw DontException(origin);
            }

            public override TFixture visitImportNUnit<TFixture>(TestPosition position) 
            {
                throw DontException(SpecElement.importNUnit);
            }

            public InvalidOperationException DontException(SpecElement innerSpecElement)
            {
                return new InvalidOperationException("Called " + innerSpecElement + "() within " + SpecElement + "().");
            }
        }
    }
}
