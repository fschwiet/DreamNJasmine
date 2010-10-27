using System;
using System.Collections.Generic;
using System.Linq;
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
            if (!type.IsSubclassOf(typeof (NJasmineFixture)))
                return false;

            if (!type.IsPublic)
                return false;

            if (type.GetConstructor(new Type[0]) == null)
                return false;

            return true;
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

            Exception exception = null;

            try
            {
                _fixture.Tests();
            }
            catch (Exception e)
            {
                exception = e;
            }

            if (exception == null)
            {
                foreach (var sibling in _siblingTests)
                    rootTest.Add(sibling);
            }
            else
            {
                rootTest.Add(new NJasmineInvalidTestSuite("Exception at top level", exception, new TestPosition(0)));
            }

            _fixture.ClearVisitor();

            _siblingTests = null;
            _parentTest = null;
            _fixture = null;

            NUnitFramework.ApplyCommonAttributes(type.GetCustomAttributes(false).Cast<Attribute>().ToArray(), rootTest);

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
                newSuite = new NJasmineInvalidTestSuite("Exception within describe", exceptionSeen, position);
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
