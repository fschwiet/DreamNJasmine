using System;
using System.Linq.Expressions;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    public abstract class GivenWhenThenFixture : SpecificationFixture
    {
        public GivenWhenThenFixture()
        {}

        protected GivenWhenThenFixture(SkeleFixture fixture)
            : base(fixture)
        {}

        public void given(string givenPhrase, Action specification)
        {
            _skeleFixture.ExtendSpec(s => s.visitFork(SpecElement.given, "given " + givenPhrase, specification));
        }

        public void when(string whenPhrase, Action specification)
        {
            _skeleFixture.ExtendSpec(s => s.visitFork(SpecElement.when, "when " + whenPhrase, specification));
        }

        public void then(string thenPhrase, Action test)
        {
            _skeleFixture.ExtendSpec(s => s.visitTest(SpecElement.then, "then " + thenPhrase, test));
        }

        public void then(string thenPhrase)
        {
            _skeleFixture.ExtendSpec(s => s.visitTest(SpecElement.then, "then " + thenPhrase, null));
        }

        public void cleanup(Action cleanup)
        {
            _skeleFixture.ExtendSpec(s => s.visitAfterEach(SpecElement.cleanup, cleanup));
        }

        public void arrange(Action arrangeAction)
        {
            _skeleFixture.ExtendSpec(s => s.visitBeforeEach(SpecElement.arrange, null, delegate() { arrangeAction(); return (string)null; }));
        }

        public T arrange<T>(Func<T> arrangeAction)
        {
            T result = default(T);
            _skeleFixture.ExtendSpec(s => result = s.visitBeforeEach(SpecElement.arrange, null, arrangeAction));
            return result;
        }

        public void expect(Expression<Func<bool>> expectation)
        {
            PowerAssert.PAssert.IsTrue(expectation);
        }

        public void beforeAll(Action action)
        {
            _skeleFixture.ExtendSpec(s =>s.visitBeforeAll<string>(SpecElement.beforeAll, delegate {
                action();
                return (string)null;
            }));
        }

        public T beforeAll<T>(Func<T> action)
        {
            T result = default(T);
            _skeleFixture.ExtendSpec(s => result = s.visitBeforeAll(SpecElement.beforeAll, action));
            return result;
        }

        public void afterAll(Action action)
        {
            _skeleFixture.ExtendSpec(s => s.visitAfterAll(SpecElement.afterAll, action));
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return NUnitFixtureDriver.IncludeFixture<TFixture>(_skeleFixture);
        }

        public void ignoreBecause(string theTestRequiresIt)
        {
        }
    }
}