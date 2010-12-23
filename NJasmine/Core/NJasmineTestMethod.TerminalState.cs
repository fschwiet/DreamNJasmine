using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core
{
    public partial class NJasmineTestMethod
    {
        public class TerminalState : DescribeState
        {
            readonly SpecMethod _specMethod;
            readonly INJasmineFixturePositionVisitor _originalVisitor;

            public TerminalState(NJasmineTestMethod subject, SpecMethod specMethod) : base(subject)
            {
                _specMethod = specMethod;
            }

            public override void visitDescribe(string description, Action action, TestPosition position)
            {
                throw DontException(SpecMethod.describe);
            }

            public override void visitBeforeEach(Action action, TestPosition position)
            {
                throw DontException(SpecMethod.beforeEach);
            }

            public override void visitAfterEach(Action action, TestPosition position)
            {
                throw DontException(SpecMethod.afterEach);
            }

            public override void visitIt(string description, Action action, TestPosition position)
            {
                throw DontException(SpecMethod.it);
            }

            public override TFixture visitImportNUnit<TFixture>(TestPosition position) 
            {
                throw DontException(SpecMethod.importNUnit);
            }

            //public override TArranged visitArrange<TArranged>(string description, IEnumerable<Func<TArranged>> factories, TestPosition position)
            //{
            //    throw DontException(SpecMethod.arrange);
            //}

            InvalidOperationException DontException(SpecMethod innerSpecMethod)
            {
                return new InvalidOperationException("Called " + innerSpecMethod + "() within " + _specMethod + "().");
            }
        }
    }
}
