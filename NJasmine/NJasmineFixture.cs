using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core;
using NJasmine.FixtureVisitor;
using NUnit.Core;
using NUnit.Framework;
using Should.Fluent;
using Should.Fluent.Model;
using Assert = Should.Core.Assertions.Assert;

namespace NJasmine
{
    public abstract class NJasmineFixture : ExpectationsFixture
    {
        INJasmineFixtureVisitor _visitor = new DoNothingFixtureVisitor();
        readonly Stack<INJasmineFixtureVisitor> _visitorStack = new Stack<INJasmineFixtureVisitor>();
        readonly Dictionary<TestPosition, object> _importedNUnitFixtures = new Dictionary<TestPosition, object>();

        public void PushVisitor(INJasmineFixtureVisitor visitor)
        {
            _visitorStack.Push(_visitor);

            _visitor = visitor;
        }

        public void PopVisitor()
        {
            _visitor = _visitorStack.Pop();
        }

        public void ClearVisitor()
        {
            _visitor = new DoNothingFixtureVisitor();
        }

        public abstract void Tests();

        protected void describe(string description, Action action)
        {
            _visitor.visitDescribe(description, action);
        }

        protected void beforeEach(Action action)
        {
            _visitor.visitBeforeEach(action);
        }

        protected void afterEach(Action action)
        {
            _visitor.visitAfterEach(action);
        }

        protected void it(string description, Action action)
        {
            _visitor.visitIt(description, action);
        }

        protected TFixture importNUnit<TFixture>() where TFixture: class, new()
        {
            return _visitor.visitImportNUnit<TFixture>();
        }

        protected void ignore(Action action)
        {
        }

        protected void ignore(string shouldntRun, Action action)
        {
        }

        public void SetNUnitFixture(TestPosition position, object fixture)
        {
            _importedNUnitFixtures[position] = fixture;
        }

        public object GetNUnitFixture(TestPosition position)
        {
            return _importedNUnitFixtures[position];
        }

        public void RunOneTimeSetup()
        {
            foreach (var fixture in _importedNUnitFixtures.Values)
            {
                foreach(var method in Reflect.GetMethodsWithAttribute(fixture.GetType(), NUnitFramework.FixtureSetUpAttribute, true))
                {
                    method.Invoke(fixture, new object[] {});
                }
            }
        }

        public void RunOneTimeTearDown()
        {
            foreach (var fixture in _importedNUnitFixtures.Values)
            {
                foreach (var method in Reflect.GetMethodsWithAttribute(fixture.GetType(), NUnitFramework.FixtureTearDownAttribute, true)
                    .Reverse())
                {
                    method.Invoke(fixture, new object[] { });
                }
            }
        }

        public void RunSetup(TestPosition position)
        {
            List<object> fixtures = GetFixturesInScopeFor(position);

            foreach (var fixture in fixtures)
            {
                foreach (var method in
                        Reflect.GetMethodsWithAttribute(fixture.GetType(), NUnitFramework.SetUpAttribute, true))
                {
                    method.Invoke(fixture, new object[] { });
                }
            }
        }

        public void RunTearDown(TestPosition position)
        {
            List<object> fixtures = GetFixturesInScopeFor(position);
            fixtures.Reverse();

            foreach (var fixture in fixtures)
            {
                foreach (var method in
                        Reflect.GetMethodsWithAttribute(fixture.GetType(), NUnitFramework.TearDownAttribute, true)
                        .Reverse())
                {
                    method.Invoke(fixture, new object[] { });
                }
            }
        }

        List<object> GetFixturesInScopeFor(TestPosition position)
        {
            List<object> fixtures = new List<object>();

            foreach (var fixturePosition in _importedNUnitFixtures.Keys)
            {
                var fixture = _importedNUnitFixtures[fixturePosition];

                if (fixturePosition.IsInScopeFor(position))
                    fixtures.Add(fixture);
            }
            return fixtures;
        }
    }
}
