using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    class NJasmineTestSuite : TestSuite, INJasmineTest, INJasmineFixturePositionVisitor
    {
        readonly NJasmineFixture _fixture;
        readonly TestPosition _position;
        readonly NUnitFixtureCollection _nunitImports;
        readonly List<Test> _accumulatedDescendants;
        readonly List<string> _globallyAccumulatedTestNames;

        bool _haveReachedAnIt;

        public NJasmineTestSuite(NJasmineFixture fixture, string baseName, string name, TestPosition position, NUnitFixtureCollection parentNUnitImports, List<string> globallyAccumulatedTestNames) 
            : base(baseName, name)
        {
            _fixture = fixture;
            _position = position;
            _nunitImports = new NUnitFixtureCollection(parentNUnitImports);
            _globallyAccumulatedTestNames = globallyAccumulatedTestNames;

            _accumulatedDescendants = new List<Test>();
            _haveReachedAnIt = false;

            maintainTestOrder = true;
        }

        public TestPosition Position
        {
            get { return _position; }
        }

        public void BuildSuite(Action action)
        {
            Exception exception = null;

            _fixture.PushVisitor(new VisitorPositionAdapter(_position.GetFirstChildPosition(), this));

            try
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    exception = e;
                }

                if (exception == null)
                {
                    foreach (var sibling in _accumulatedDescendants)
                        this.Add(sibling);
                }
                else
                {
                    var nJasmineInvalidTestSuite = new NJasmineInvalidTestSuite(exception, _position);

                    nJasmineInvalidTestSuite.TestName.FullName = TestName.FullName;
                    nJasmineInvalidTestSuite.TestName.Name = TestName.Name;
                    MakeNameUnique(nJasmineInvalidTestSuite);
                    NameTest(nJasmineInvalidTestSuite, TestName.FullName, "invalid describe");

                    this.Add(nJasmineInvalidTestSuite);
                }
            }
            finally
            {
                _fixture.PopVisitor();
            }
        }

        private void MakeNameUnique(TestMethod test)
        {
            var name = test.TestName.FullName;

            if (_globallyAccumulatedTestNames.Contains(name))
            {
                var nextIndex = 1;
                string suffix;
                string nextName;

                do
                {
                    suffix = "`" + ++nextIndex;
                    nextName = name + suffix;
                } while (_globallyAccumulatedTestNames.Contains(nextName));


                test.TestName.Name = test.TestName.Name + suffix;
                test.TestName.FullName = test.TestName.FullName + suffix;
            }

            _globallyAccumulatedTestNames.Add(test.TestName.FullName);
        }

        private void NameTest(TestMethod test, string baseName, string name)
        {
            test.TestName.FullName = baseName + " " + name;
            test.TestName.Name = name;

            MakeNameUnique(test);
        }
        
        public void visitDescribe(string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var nJasmineUnimplementedTestMethod = new NJasmineUnimplementedTestMethod(position);

                NameTest(nJasmineUnimplementedTestMethod, TestName.FullName, description);

                _accumulatedDescendants.Add(nJasmineUnimplementedTestMethod);
            }
            else
            {
                var describeSuite = new NJasmineTestSuite(_fixture, TestName.FullName, description, position, _nunitImports, _globallyAccumulatedTestNames);

                describeSuite.BuildSuite(action);

                _accumulatedDescendants.Add(describeSuite);
            }
        }

        public void visitBeforeEach(Action action, TestPosition position)
        {
            if (_haveReachedAnIt)
                throw WrongMethodAfterItMethod(SpecMethod.beforeEach);
        }

        public void visitAfterEach(Action action, TestPosition position)
        {
            if (_haveReachedAnIt)
                throw WrongMethodAfterItMethod(SpecMethod.afterEach);
        }

        public void visitIt(string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var nJasmineUnimplementedTestMethod = new NJasmineUnimplementedTestMethod(position);

                NameTest(nJasmineUnimplementedTestMethod, TestName.FullName, description);

                _accumulatedDescendants.Add(nJasmineUnimplementedTestMethod);
            }
            else
            {
                var testMethod = new NJasmineTestMethod(_fixture, position, _nunitImports);

                NameTest(testMethod, TestName.FullName, description);

                _accumulatedDescendants.Add(testMethod);
            }

            _haveReachedAnIt = true;
        }

        public TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new()
        {
            if (_haveReachedAnIt)
                throw WrongMethodAfterItMethod(SpecMethod.importNUnit);

            _nunitImports.AddFixture(position, typeof(TFixture));

            return null;
        }

        public TArranged visitArrange<TArranged>(TestPosition position) where TArranged : class, new()
        {
            if (_haveReachedAnIt)
                throw WrongMethodAfterItMethod(SpecMethod.arrange);

            return null;
        }

        public TArranged visitArrange<TArranged>(Func<TArranged> factory, TestPosition position)
        {
            if (_haveReachedAnIt)
                throw WrongMethodAfterItMethod(SpecMethod.arrange);

            return default(TArranged);
        }

        protected override void DoOneTimeSetUp(TestResult suiteResult)
        {
            _nunitImports.DoOnetimeSetUp();
        }

        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
            _nunitImports.DoOnetimeTearDown();
        }

        InvalidOperationException WrongMethodAfterItMethod(SpecMethod innerSpecMethod)
        {
            return new InvalidOperationException("Called " + innerSpecMethod + "() after " + SpecMethod.it + "().");
        }
    }
}
