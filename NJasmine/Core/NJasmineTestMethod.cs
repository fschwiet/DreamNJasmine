using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    class NJasmineTestMethod : TestMethod, INJasmineTest, INJasmineFixturePositionVisitor
    {
        readonly NJasmineFixture _fixture;
        readonly TestPosition _position;

        List<Action> _setups = new List<Action>();
        List<Action> _teardowns = new List<Action>();
        Action _actual;

        public NJasmineTestMethod(MethodInfo method, NJasmineFixture fixture, TestPosition position) : base(method)
        {
            _fixture = fixture;
            _position = position;
        }

        public static NJasmineTestMethod Create(NJasmineFixture fixture, TestPosition position)
        {
            NJasmineTestMethod result = null;

            Action testMethod = delegate()
            {
                result.Run();
            };

            result = new NJasmineTestMethod(testMethod.Method, fixture, position);

            return result;
        }

        public TestPosition Position
        {
            get { return _position; }
        }

        public void Run()
        {
            _fixture.SetVisitor(new VisitorPositionAdapter(this));
        }

        public void visitDescribe(string description, Action action, TestPosition position)
        {
            throw new NotImplementedException();
        }

        public void visitBeforeEach(Action action, TestPosition position)
        {
            throw new NotImplementedException();
        }

        public void visitAfterEach(Action action, TestPosition position)
        {
            throw new NotImplementedException();
        }

        public void visitIt(string description, Action action, TestPosition position)
        {
            throw new NotImplementedException();
        }
    }
}
