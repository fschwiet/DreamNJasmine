using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    class NJasmineTestSuite : TestSuite, INJasmineTest, INJasmineFixturePositionVisitor
    {
        readonly NJasmineFixture _fixture;
        readonly string _parentSuiteName;
        readonly string _name;
        readonly TestPosition _position;
        List<Test> _accumulatedDescendants = new List<Test>();

        public NJasmineTestSuite(NJasmineFixture fixture, string parentSuiteName, string name, TestPosition position) 
            : base(parentSuiteName, name)
        {
            _fixture = fixture;
            _parentSuiteName = parentSuiteName;
            _name = name;
            _position = position;

            base.fixtureSetUpMethods = new MethodInfo[] { ((Action)this.FixtureSetup).Method };
            base.fixtureTearDownMethods= new MethodInfo[] { ((Action)this.FixtureSetup).Method };
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
                    this.Add(new NJasmineInvalidTestSuite("Exception thrown within test definition", exception, _position));
                }
            }
            finally
            {
                _fixture.PopVisitor();
            }
        }
        
        public void visitDescribe(string description, Action action, TestPosition position)
        {
            var describeSuite = new NJasmineTestSuite(_fixture, _name, description, position);

            describeSuite.BuildSuite(action);

            _accumulatedDescendants.Add(describeSuite);
        }

        public void visitBeforeEach(Action action, TestPosition position)
        {
        }

        public void visitAfterEach(Action action, TestPosition position)
        {
        }

        public void visitIt(string description, Action action, TestPosition position)
        {
            var testMethod = new NJasmineTestMethod(_fixture, position);

            testMethod.TestName.Name = description;

            _accumulatedDescendants.Add(testMethod);
        }

        public TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new()
        {
            TFixture fixture = new TFixture();

            _fixture.SetNUnitFixture(position, fixture);

            return fixture;
        }

        public void FixtureSetup()
        {
            _fixture.RunOneTimeSetup();
        }

        public void FixtureTearDown()
        {
            _fixture.RunOneTimeTearDown();
        }
    }
}
