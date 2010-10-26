using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.FixtureVisitor
{
    public class DontVisitor : INJasmineFixtureVisitor
    {
        readonly SpecMethod _specMethod;

        public enum SpecMethod
        {
            describe,
            beforeEach,
            afterEach,
            it
        }

        public DontVisitor(SpecMethod specMethod)
        {
            _specMethod = specMethod;
        }

        public void visitDescribe(string description, Action action)
        {
            throw DontException(SpecMethod.describe);
        }

        public void visitBeforeEach(Action action)
        {
            throw DontException(SpecMethod.beforeEach);
        }

        public void visitAfterEach(Action action)
        {
            throw DontException(SpecMethod.afterEach);
        }

        public void visitIt(string description, Action action)
        {
            throw DontException(SpecMethod.it);
        }

        InvalidOperationException DontException(SpecMethod innerSpecMethod)
        {
            return new InvalidOperationException("Called " + innerSpecMethod + "() within " + _specMethod + ".");
        }
    }
}
