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

        public bool CanBuildFrom(Type type)
        {
            return type.IsSubclassOf(typeof (NJasmineFixture));
        }

        public Test BuildFrom(Type type)
        {
            var instance = type.GetConstructor(new Type[0]).Invoke(new object[0]) as NJasmineFixture;

            var rootTest = new TestSuite(type.Name);

            _outerTest = rootTest;
            
            instance.SetVisitor(this);

            instance.Tests();

            instance.ClearVisitor();

            _outerTest = null;

            return rootTest;
        }

        public void visitDescribe(string description, Action action)
        {
            var currentOuter = _outerTest;
            _outerTest = new TestSuite(description);

            action();

            currentOuter.Add(_outerTest);
            _outerTest = currentOuter;
        }

        public void visitBeforeEach(Action action)
        {
        }

        public void visitAfterEach(Action action)
        {
        }

        public void visitIt(string description, Action action)
        {
            var testMethod = new NUnitTestMethod(action.Method);

            testMethod.TestName.Name = description;

            _outerTest.Add(testMethod);
        }
    }
}
