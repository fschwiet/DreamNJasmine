using System;
using NUnit.Core;
using NUnit.Core.Extensibility;

namespace NJasmine.Core
{
    [NUnitAddin(Description = "NJasmine")]
    public class NJasmineSuiteBuilder : IAddin, ISuiteBuilder, INJasmineFixtureVisitor
    {
        public bool Install(IExtensionHost host)
        {
            host.GetExtensionPoint("SuiteBuilders").Install(this);
            return true;
        }

        TestSuite _outerTest = null;
        TestPosition _nextPosition = null;

        public bool CanBuildFrom(Type type)
        {
            return type.IsSubclassOf(typeof (NJasmineFixture));
        }

        public Test BuildFrom(Type type)
        {
            var instance = type.GetConstructor(new Type[0]).Invoke(new object[0]) as NJasmineFixture;

            var rootTest = new TestSuite(type.Name);

            _outerTest = rootTest;
            _nextPosition = new TestPosition(0);

            instance.SetVisitor(this);

            instance.Tests();

            instance.ClearVisitor();

            _outerTest = null;

            return rootTest;
        }

        public void visitDescribe(string description, Action action)
        {
            TestPosition thisPosition = _nextPosition;

            var currentOuter = _outerTest;
            _outerTest = new NJasmineTestSuite(currentOuter.TestName.Name, description, thisPosition);

            _nextPosition = thisPosition.GetFirstChildPosition();

            action();

            currentOuter.Add(_outerTest);
            _outerTest = currentOuter;

            _nextPosition = thisPosition.GetNextSiblingPosition();
        }

        public void visitBeforeEach(Action action)
        {
            _nextPosition = _nextPosition.GetNextSiblingPosition();
        }

        public void visitAfterEach(Action action)
        {
            _nextPosition = _nextPosition.GetNextSiblingPosition();
        }

        public void visitIt(string description, Action action)
        {
            var testMethod = new NJasmineTestMethod(action.Method, _nextPosition);

            testMethod.TestName.Name = description;

            _outerTest.Add(testMethod);

            _nextPosition = _nextPosition.GetNextSiblingPosition();
        }
    }
}
