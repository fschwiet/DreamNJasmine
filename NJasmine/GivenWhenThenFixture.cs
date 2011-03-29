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

        public void describe(string description, Action specification)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();
            _skeleFixture.Visitor.visitFork(SpecElement.describe, description, specification, position);
            _skeleFixture.CurrentPosition = nextPosition;
        }

        public void given(string givenPhrase, Action specification)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();
            _skeleFixture.Visitor.visitFork(SpecElement.given, "given " + givenPhrase, specification, position);
            _skeleFixture.CurrentPosition = nextPosition;
        }

        public void when(string whenPhrase, Action specification)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();
            _skeleFixture.Visitor.visitFork(SpecElement.when, "when " + whenPhrase, specification, position);
            _skeleFixture.CurrentPosition = nextPosition;
        }

        public void then(string thenPhrase, Action test)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();
            _skeleFixture.Visitor.visitTest(SpecElement.then, "then " + thenPhrase, test, position);
            _skeleFixture.CurrentPosition = nextPosition;
        }

        public void then(string thenPhrase)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();
            _skeleFixture.Visitor.visitTest(SpecElement.then, "then " + thenPhrase, null, position);
            _skeleFixture.CurrentPosition = nextPosition;
        }

        public void it(string itPhrase, Action action)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();
            _skeleFixture.Visitor.visitTest(SpecElement.it, itPhrase, action, position);
            _skeleFixture.CurrentPosition = nextPosition;
        }

        public void it(string itPhrase)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();
            _skeleFixture.Visitor.visitTest(SpecElement.it, itPhrase, null, position);
            _skeleFixture.CurrentPosition = nextPosition;
        }

        public void afterEach(Action cleanup)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();
            _skeleFixture.Visitor.visitAfterEach(SpecElement.afterEach, cleanup, position);
            _skeleFixture.CurrentPosition = nextPosition;
        }

        public void cleanup(Action cleanup)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();
            _skeleFixture.Visitor.visitAfterEach(SpecElement.cleanup, cleanup, position);
            _skeleFixture.CurrentPosition = nextPosition;
        }

        public void beforeEach(Action action)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();
            _skeleFixture.Visitor.visitBeforeEach(SpecElement.beforeEach, delegate()
            {
                action();
                return (string)null;
            }, position);

            _skeleFixture.CurrentPosition = nextPosition;
        }

        public void arrange(Action arrangeAction)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();

            _skeleFixture.Visitor.visitBeforeEach(SpecElement.arrange, delegate()
            {
                arrangeAction();
                return (string)null;
            }, position);

            _skeleFixture.CurrentPosition = nextPosition;
        }

        public T arrange<T>(Func<T> arrangeAction)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();

            T result = _skeleFixture.Visitor.visitBeforeEach(SpecElement.arrange, arrangeAction, position);

            _skeleFixture.CurrentPosition = nextPosition;

            return result;
        }

        public TArranged arrange<TArranged>() where TArranged : class, new()        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();
            Func<TArranged> factory = delegate { return new TArranged(); };            var result = _skeleFixture.Visitor.visitBeforeEach(SpecElement.arrange, factory, position);

            _skeleFixture.CurrentPosition = nextPosition;

            return result;
        }

        public void beforeAll(Action action)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();

            _skeleFixture.Visitor.visitBeforeAll<string>(SpecElement.beforeAll, delegate
            {
                action();
                return (string) null;
            }, position);

            _skeleFixture.CurrentPosition = nextPosition;
        }

        public T beforeAll<T>(Func<T> action)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();

            var result = _skeleFixture.Visitor.visitBeforeAll(SpecElement.beforeAll, action, position);

            _skeleFixture.CurrentPosition = nextPosition;

            return result;
        }

        public void afterAll(Action action)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();

            _skeleFixture.Visitor.visitAfterAll(SpecElement.afterAll, action, position);

            _skeleFixture.CurrentPosition = nextPosition;
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return NUnitFixtureDriver.IncludeFixture<TFixture>(this);
        }

        public void ignoreBecause(string reason)
        {
            var position = _skeleFixture.CurrentPosition;
            var nextPosition = _skeleFixture.CurrentPosition.GetNextSiblingPosition();
            _skeleFixture.CurrentPosition = _skeleFixture.CurrentPosition.GetFirstChildPosition();

            _skeleFixture.Visitor.visitIgnoreBecause(reason, position);
            _skeleFixture.CurrentPosition = nextPosition;
        }
    }
}