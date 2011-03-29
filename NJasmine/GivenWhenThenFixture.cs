using System;
using System.Linq.Expressions;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    [NUnit.Framework.Explicit]
    public abstract class GivenWhenThenFixture : SpecificationFixture
    {
        public void describe(string description, Action specification)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            base.Visitor.visitFork(SpecElement.describe, description, specification, position);
            base.CurrentPosition = nextPosition;
        }

        public void given(string givenPhrase, Action specification)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            base.Visitor.visitFork(SpecElement.given, "given " + givenPhrase, specification, position);
            base.CurrentPosition = nextPosition;
        }

        public void when(string whenPhrase, Action specification)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            base.Visitor.visitFork(SpecElement.when, "when " + whenPhrase, specification, position);
            base.CurrentPosition = nextPosition;
        }

        public void then(string thenPhrase, Action test)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            base.Visitor.visitTest(SpecElement.then, "then " + thenPhrase, test, position);
            base.CurrentPosition = nextPosition;
        }

        public void then(string thenPhrase)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            base.Visitor.visitTest(SpecElement.then, "then " + thenPhrase, null, position);
            base.CurrentPosition = nextPosition;
        }

        public void it(string itPhrase, Action action)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            base.Visitor.visitTest(SpecElement.it, itPhrase, action, position);
            base.CurrentPosition = nextPosition;
        }

        public void it(string itPhrase)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            base.Visitor.visitTest(SpecElement.it, itPhrase, null, position);
            base.CurrentPosition = nextPosition;
        }

        public void afterEach(Action cleanup)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            base.Visitor.visitAfterEach(SpecElement.afterEach, cleanup, position);
            base.CurrentPosition = nextPosition;
        }

        public void cleanup(Action cleanup)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            base.Visitor.visitAfterEach(SpecElement.cleanup, cleanup, position);
            base.CurrentPosition = nextPosition;
        }

        public void beforeEach(Action action)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            base.Visitor.visitBeforeEach(SpecElement.beforeEach, delegate()
            {
                action();
                return (string)null;
            }, position);

            base.CurrentPosition = nextPosition;
        }

        public void arrange(Action arrangeAction)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();

            base.Visitor.visitBeforeEach(SpecElement.arrange, delegate()
            {
                arrangeAction();
                return (string)null;
            }, position);

            base.CurrentPosition = nextPosition;
        }

        public T arrange<T>(Func<T> arrangeAction)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();

            T result = base.Visitor.visitBeforeEach(SpecElement.arrange, arrangeAction, position);

            base.CurrentPosition = nextPosition;

            return result;
        }

        public TArranged arrange<TArranged>() where TArranged : class, new()        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            Func<TArranged> factory = delegate { return new TArranged(); };            var result = base.Visitor.visitBeforeEach(SpecElement.arrange, factory, position);

            base.CurrentPosition = nextPosition;

            return result;
        }

        public void beforeAll(Action action)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();

            base.Visitor.visitBeforeAll<string>(SpecElement.beforeAll, delegate
            {
                action();
                return (string) null;
            }, position);

            base.CurrentPosition = nextPosition;
        }

        public T beforeAll<T>(Func<T> action)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();

            var result = base.Visitor.visitBeforeAll(SpecElement.beforeAll, action, position);

            base.CurrentPosition = nextPosition;

            return result;
        }

        public void afterAll(Action action)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();

            base.Visitor.visitAfterAll(SpecElement.afterAll, action, position);

            base.CurrentPosition = nextPosition;
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return NUnitFixtureDriver.IncludeFixture<TFixture>(this);
        }

        public void ignoreBecause(string reason)
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();

            base.Visitor.visitIgnoreBecause(reason, position);
            base.CurrentPosition = nextPosition;
        }
    }
}