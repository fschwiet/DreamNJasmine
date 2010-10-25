using System;
using NJasmine.FixtureVisitor;
using NUnit.Core;
using NUnit.Core.Extensibility;

namespace NJasmine.Core
{
    [NUnitAddin(Description = "NJasmine")]
    public class NJasmineSuiteBuilder : IAddin, ISuiteBuilder, INJasmineFixturePositionVisitor
    {
        public bool Install(IExtensionHost host)
        {
            host.GetExtensionPoint("SuiteBuilders").Install(this);
            return true;
        }

        NJasmineFixture _fixture = null;
        TestSuite _outerTest = null;

        public bool CanBuildFrom(Type type)
        {
            return type.IsSubclassOf(typeof (NJasmineFixture));
        }

        public Test BuildFrom(Type type)
        {
            _fixture = type.GetConstructor(new Type[0]).Invoke(new object[0]) as NJasmineFixture;

            var rootTest = new TestSuite(type.Name);

            _outerTest = rootTest;

            _fixture.SetVisitor(new VisitorPositionAdapter(this));

            _fixture.Tests();

            _fixture.ClearVisitor();

            _outerTest = null;
            _fixture = null;

            return rootTest;
        }

        public void visitDescribe(string description, Action action, TestPosition position)
        {
            var currentOuter = _outerTest;
            _outerTest = new NJasmineTestSuite(currentOuter.TestName.Name, description, position);

            action();

            currentOuter.Add(_outerTest);
            _outerTest = currentOuter;
        }

        public void visitBeforeEach(Action action, TestPosition position)
        {
        }

        public void visitAfterEach(Action action, TestPosition position)
        {
        }

        public void visitIt(string description, Action action, TestPosition position)
        {
            var testMethod = NJasmineTestMethod.Create(_fixture, position);

            testMethod.TestName.Name = description;

            _outerTest.Add(testMethod);
        }
    }
}
