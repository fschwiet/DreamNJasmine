using System;
using System.Linq.Expressions;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    [NUnit.Framework.Explicit]
    public abstract class GivenWhenThenFixture : SpecificationFixture
    {
        public GivenWhenThenFixture()
        {}

        protected GivenWhenThenFixture(SkeleFixture fixture)
            : base(fixture)
        {}

        public void given(string givenPhrase, Action specification)
        {
            _skeleFixture.Visitor.visitFork(SpecElement.given, "given " + givenPhrase, specification);
        }

        public void when(string whenPhrase, Action specification)
        {
            _skeleFixture.Visitor.visitFork(SpecElement.when, "when " + whenPhrase, specification);
        }

        public void then(string thenPhrase, Action test)
        {
            _skeleFixture.Visitor.visitTest(SpecElement.then, "then " + thenPhrase, test);
        }

        public void then(string thenPhrase)
        {
            _skeleFixture.Visitor.visitTest(SpecElement.then, "then " + thenPhrase, null);
        }

        public void cleanup(Action cleanup)
        {
            _skeleFixture.Visitor.visitAfterEach(SpecElement.cleanup, cleanup);
        }

        public void arrange(Action arrangeAction)
        {
            _skeleFixture.Visitor.visitBeforeEach(SpecElement.arrange, delegate()
            {
                arrangeAction();
                return (string) null;
            });
        }

        public T arrange<T>(Func<T> arrangeAction)
        {
            T result = default(T);
            result = _skeleFixture.Visitor.visitBeforeEach(SpecElement.arrange, arrangeAction);
            return result;
        }

        public void beforeAll(Action action)
        {
            _skeleFixture.Visitor.visitBeforeAll<string>(SpecElement.beforeAll, delegate
            {
                action();
                return (string) null;
            });
        }

        public T beforeAll<T>(Func<T> action)
        {
            return  _skeleFixture.Visitor.visitBeforeAll(SpecElement.beforeAll, action);
        }

        public void afterAll(Action action)
        {
            _skeleFixture.Visitor.visitAfterAll(SpecElement.afterAll, action);
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return NUnitFixtureDriver.IncludeFixture<TFixture>(_skeleFixture);
        }

        public void ignoreBecause(string reason)
        {
            _skeleFixture.Visitor.visitIgnoreBecause(reason);
        }
    }
}