using System;
using System.Linq.Expressions;
using System.Threading;
using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    [NUnit.Framework.Explicit]
    public abstract class GivenWhenThenFixture : SpecificationFixture
    {
        //
        //  When a test failure occurs, the developer must consider the callstack 
        //  to find the error.  Every layer of nesting makes that more difficult.
        //  So I go to extra effort to reduce the callstack during re-entry, using
        //  PositionContext to keep things DRY.
        //

        public class PositionContext : IDisposable
        {
            public readonly TestPosition Position;

            private readonly Action _onFinish;

            public PositionContext(TestPosition position, Action onFinish)
            {
                Position = position;
                _onFinish = onFinish;
            }

            public void Dispose()
            {
                _onFinish();
            }
        }

        private PositionContext SetPositionForNestedReentry_then_Restore_and_Advance_for_Next()
        {
            var position = base.CurrentPosition;
            var nextPosition = base.CurrentPosition.GetNextSiblingPosition();
            base.CurrentPosition = base.CurrentPosition.GetFirstChildPosition();
            return new PositionContext(position,delegate
            {
                base.CurrentPosition = nextPosition;
            });
        }

        public void describe(string description, Action specification)
        {
            using(var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitFork(SpecElement.describe, description, specification, context.Position);
            }
        }

        public void given(string givenPhrase, Action specification)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitFork(SpecElement.given, "given " + givenPhrase, specification, context.Position);
            }
        }

        public void when(string whenPhrase, Action specification)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitFork(SpecElement.when, "when " + whenPhrase, specification, context.Position);
            }
        }

        public void then(string thenPhrase, Action test)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitTest(SpecElement.then, "then " + thenPhrase, test, context.Position);
            }
        }

        public void then(string thenPhrase)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitTest(SpecElement.then, "then " + thenPhrase, null, context.Position);
            }
        }

        public void it(string itPhrase, Action action)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitTest(SpecElement.it, itPhrase, action, context.Position);
            }
        }

        public void it(string itPhrase)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitTest(SpecElement.it, itPhrase, null, context.Position);
            }
        }

        public void afterEach(Action cleanup)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitAfterEach(SpecElement.afterEach, cleanup, context.Position);
            }
        }

        public void cleanup(Action cleanup)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitAfterEach(SpecElement.cleanup, cleanup, context.Position);
            }
        }

        public void beforeEach(Action action)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitBeforeEach(SpecElement.beforeEach, delegate()
                    {
                        action();
                        return (string)null;
                    }, 
                    context.Position);
            }
        }

        public void arrange(Action arrangeAction)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitBeforeEach(SpecElement.arrange, delegate()
                    {
                        arrangeAction();
                        return (string)null;
                    }, 
                    context.Position);
            }
        }

        public T arrange<T>(Func<T> arrangeAction)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                return base.Visitor.visitBeforeEach(SpecElement.arrange, arrangeAction, context.Position);
            }
        }

        public TArranged arrange<TArranged>() where TArranged : class, new()
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                Func<TArranged> factory = delegate { return new TArranged(); };

                return base.Visitor.visitBeforeEach(SpecElement.arrange, factory, context.Position);
            }
        }


        public void beforeAll(Action action)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitBeforeAll<string>(SpecElement.beforeAll, delegate
                    {
                        action();
                        return (string)null;
                    }, 
                    context.Position);
            }
        }

        public T beforeAll<T>(Func<T> action)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                return base.Visitor.visitBeforeAll(SpecElement.beforeAll, action, context.Position);
            }
        }

        public void afterAll(Action action)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitAfterAll(SpecElement.afterAll, action, context.Position);
            }
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return NUnitFixtureDriver.IncludeFixture<TFixture>(this);
        }

        public void ignoreBecause(string reason)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitIgnoreBecause(reason, context.Position);
            }
        }

        public void expect(Expression<Func<bool>> expectation)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitExpect(expectation, context.Position);
            }
        }

        private int _msWaitMax = 1000;
        private int _msWaitIncrement = 250;

        public void setWaitTimeouts(int msWaitMax, int msWaitIncrement)
        {
            var originalWaitMax = msWaitMax;
            var originalWaitIncrement = msWaitIncrement;

            _msWaitMax = msWaitMax;
            _msWaitIncrement = Math.Min(msWaitIncrement, 1);

            cleanup(delegate
            {
                _msWaitMax = originalWaitMax;
                _msWaitIncrement = originalWaitIncrement;
            });
        }

        public void waitUntil(Expression<Func<bool>> expectation, int? msWaitMax = null, int? msWaitIncrement = null)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitWaitUntil(expectation, msWaitMax ?? _msWaitMax, msWaitIncrement ?? _msWaitIncrement, context.Position);
            }
        }
    }
}