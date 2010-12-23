using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core
{
    public partial class NJasmineTestMethod
    {
        public class ArrangeState : DescribeState
        {
            protected readonly SpecMethod _specMethod;

            public ArrangeState(NJasmineTestMethod subject, SpecMethod specMethod)
                : base(subject)
            {
                _specMethod = specMethod;
            }

            public override void visitDescribe(string description, Action action, TestPosition position)
            {
                throw DontException(SpecMethod.describe);
            }

            public override void visitIt(string description, Action action, TestPosition position)
            {
                throw DontException(SpecMethod.it);
            }

            public override TFixture visitImportNUnit<TFixture>(TestPosition position) 
            {
                throw DontException(SpecMethod.importNUnit);
            }

            public InvalidOperationException DontException(SpecMethod innerSpecMethod)
            {
                return new InvalidOperationException("Called " + innerSpecMethod + "() within " + _specMethod + "().");
            }
        }
    }
}
