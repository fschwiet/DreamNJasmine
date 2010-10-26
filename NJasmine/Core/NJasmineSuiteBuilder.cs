using System;
using System.Collections.Generic;
using NJasmine.FixtureVisitor;
using NUnit.Core;
using NUnit.Core.Extensibility;

namespace NJasmine.Core
{
    [NUnitAddin(Description = "NJasmine", Type = ExtensionType.Core)]
    public class NJasmineSuiteBuilder : IAddin, ISuiteBuilder, INJasmineFixturePositionVisitor
    {
        public bool Install(IExtensionHost host)
        {
            host.GetExtensionPoint("SuiteBuilders").Install(this);
            return true;
        }

        public bool CanBuildFrom(Type type)
        {
            return type.IsSubclassOf(typeof (NJasmineFixture)) && type.IsPublic;
        }

        NJasmineFixture _fixture = null;
        TestSuite _parentTest = null;
        List<Test> _siblingTests = null;

        public Test BuildFrom(Type type)
        {
            _fixture = type.GetConstructor(new Type[0]).Invoke(new object[0]) as NJasmineFixture;

            var rootTest = new TestSuite(type.Namespace, type.Name);

            _parentTest = rootTest;
            _siblingTests = new List<Test>();

            _fixture.SetVisitor(new VisitorPositionAdapter(this));

            try
            {
                _fixture.Tests();
            }
            catch (Exception e)
            {
                rootTest.Add(new NJasmineInvalidTestSuite("Exception at top level", e));

                return rootTest;
            }

            _fixture.ClearVisitor();

            foreach (var sibling in _siblingTests)
                rootTest.Add(sibling);

            _siblingTests = null;
            _parentTest = null;
            _fixture = null;

            return rootTest;
        }

        public void visitDescribe(string description, Action action, TestPosition position)
        {
            var currentParent = _parentTest;
            var currentSiblings = _siblingTests;

            _parentTest = new NJasmineTestSuite(currentParent.TestName.Name, description, position);
            Test newSuite = _parentTest;

            _siblingTests = new List<Test>();

            Exception exceptionSeen = null;
            
            try
            {
                action();
            }
            catch (Exception e)
            {
                exceptionSeen = e;
            }

            if (exceptionSeen != null)
            {
                newSuite = new NJasmineInvalidTestSuite("Exception within describe", exceptionSeen);
            }
            else
            {
                foreach (var sibling in _siblingTests)
                    _parentTest.Add(sibling);
            }
            
            _parentTest = currentParent;
            _siblingTests = currentSiblings;

            _siblingTests.Add(newSuite);
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

            _siblingTests.Add(testMethod);
        }
    }
}
