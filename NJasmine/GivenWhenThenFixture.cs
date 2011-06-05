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

        private class PositionContext : IDisposable
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

        /// <summary>
        /// Branches the current test specification
        /// </summary>
        /// <param name="description">Description text used to name the test branch.</param>
        /// <param name="specification">The branched portion of the specification.</param>
        public void describe(string description, Action specification)
        {
            using(var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitFork(SpecElement.describe, description, specification, context.Position);
            }
        }

        /// <summary>
        /// Branches the current test specification
        /// </summary>
        /// <param name="description">Description text used to name the test branch -- will be prefixed with 'given'.</param>
        /// <param name="specification">The branched portion of the specification.</param>
        public void given(string description, Action specification)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitFork(SpecElement.given, "given " + description, specification, context.Position);
            }
        }

        /// <summary>
        /// Branches the current test specification.
        /// </summary>
        /// <param name="description">Description text used to name the test branch -- will be prefixed with 'when'.</param>
        /// <param name="specification">The branched portion of the specification.</param>
        public void when(string description, Action specification)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitFork(SpecElement.when, "when " + description, specification, context.Position);
            }
        }

        /// <summary>
        /// Adds a test.
        /// </summary>
        /// <param name="description">The description that names the test -- will be prefixed with 'then'.</param>
        /// <param name="test">The test implementation.</param>
        public void then(string description, Action test)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitTest(SpecElement.then, "then " + description, test, context.Position);
            }
        }

        /// <summary>
        /// Adds an unimplemented test.
        /// </summary>
        /// <param name="description">The description that names the test -- will be prefixed with 'then'.</param>
        public void then(string description)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitTest(SpecElement.then, "then " + description, null, context.Position);
            }
        }

        /// <summary>
        /// Adds a test
        /// </summary>
        /// <param name="description">The description that names the test.</param>
        /// <param name="action">The test implementation.</param>
        public void it(string description, Action action)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitTest(SpecElement.it, description, action, context.Position);
            }
        }

        /// <summary>
        /// Adds an unimplemented test.
        /// </summary>
        /// <param name="description">The description that names the test.</param>
        public void it(string description)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitTest(SpecElement.it, description, null, context.Position);
            }
        }

        /// <summary>
        /// Adds cleanup code to be ran after each test in the following context.
        /// </summary>
        /// <param name="cleanup">The cleanup code.</param>
        public void afterEach(Action cleanup)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitAfterEach(SpecElement.afterEach, cleanup, context.Position);
            }
        }

        /// <summary>
        /// Adds cleanup code to be ran after each test in the following context.
        /// </summary>
        /// <param name="cleanup">The cleanup code.</param>
        public void cleanup(Action cleanup)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitAfterEach(SpecElement.cleanup, cleanup, context.Position);
            }
        }

        /// <summary>
        /// Adds initialization code to be before after each test in the following context.
        /// </summary>
        /// <param name="action">The initialization code.</param>
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

        /// <summary>
        /// Adds initialization code to be before after each test in the following context.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        public void arrange(Action action)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitBeforeEach(SpecElement.arrange, delegate()
                {
                    action();
                    return (string)null;
                },
                    context.Position);
            }
        }

        /// <summary>
        /// Functionally the same as arrange(), has the semantics of exercising the sut.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        public void act(Action action)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitBeforeEach(SpecElement.act, delegate()
                {
                    action();
                    return (string)null;
                },
                    context.Position);
            }
        }

        /// <summary>
        /// Adds initialization code to be before after each test in the following context.
        /// A return value can be used in the remainder of the test.
        /// If that return value supports IDisposable, it will be disposed when the test is done.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        /// <returns>The result of the initialization code.</returns>
        public T arrange<T>(Func<T> arrangeAction)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                return base.Visitor.visitBeforeEach(SpecElement.arrange, arrangeAction, context.Position);
            }
        }

        /// <summary>
        /// Adds initialization code to instantiate a class before after each test in the following context.
        /// If that class supports IDisposable, it will be disposed when the test is done.
        /// </summary>
        /// <typeparam name="TArranged">The class to be created.</typeparam>
        /// <returns>The class instance created.</returns>
        public TArranged arrange<TArranged>() where TArranged : class, new()
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                Func<TArranged> factory = delegate { return new TArranged(); };

                return base.Visitor.visitBeforeEach(SpecElement.arrange, factory, context.Position);
            }
        }

        /// <summary>
        /// Adds initialization code to be ran once before all tests in the following context.
        /// </summary>
        /// <param name="action">The initialization code.</param>
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

        /// <summary>
        /// Adds initialization code to be ran once before all tests in the following context.
        /// The initialization code can return a value, which will be made available to every test
        /// in the following context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The initialization code.</param>
        /// <returns>The return value of the initialization code.</returns>
        public T beforeAll<T>(Func<T> action)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                return base.Visitor.visitBeforeAll(SpecElement.beforeAll, action, context.Position);
            }
        }

        /// <summary>
        /// Adds cleanup code to be ran once after all tests in the following context.
        /// </summary>
        /// <param name="action">The cleanup code.</param>
        public void afterAll(Action action)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitAfterAll(SpecElement.afterAll, action, context.Position);
            }
        }

        /// <summary>
        /// Imports an NUnit fixture, running its setup and teardown functions.
        /// </summary>
        /// <typeparam name="TFixture">The type of the NUnit fixture</typeparam>
        /// <returns>The NUnit fixture instance.</returns>
        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return NUnitFixtureDriver.IncludeFixture<TFixture>(this);
        }

        /// <summary>
        /// Indicates that the tests in the following specification context should not
        /// be ran unless the user explicitly asks for them to be ran.  Similar to
        /// NUnit's ExplicitAttribute.
        /// </summary>
        /// <param name="reason"></param>
        public void ignoreBecause(string reason)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitIgnoreBecause(SpecElement.ignore, reason, context.Position);
            }
        }

        /// <summary>
        /// Verifies a particular expecation when the tests run.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        public void expect(Expression<Func<bool>> expectation)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitExpect(SpecElement.expect, expectation, context.Position);
            }
        }

        private int _msWaitMax = 1000;
        private int _msWaitIncrement = 250;

        /// <summary>
        /// Modifies the default timeouts used by waitUntil and expectEventually.
        /// </summary>
        /// <param name="msWaitMax">The maximum time to wait, in milliseconds.</param>
        public void setWaitTimeout(int msWaitMax)
        {
            var originalWaitMax = msWaitMax;

            _msWaitMax = msWaitMax;

            cleanup(delegate
            {
                _msWaitMax = originalWaitMax;
            });
        }

        /// <summary>
        /// Modifies the default polling interval used by waitUntil and expectEventually.
        /// </summary>
        /// <param name="msWaitIncrement">The polling interval, in seconds.</param>
        public void setWaitIncrement(int msWaitIncrement)
        {
            var originalWaitIncrement = msWaitIncrement;

            _msWaitIncrement = Math.Min(msWaitIncrement, 1);

            cleanup(delegate
            {
                _msWaitIncrement = originalWaitIncrement;
            });
        }

        /// <summary>
        /// Verifies a particular expectation is true as the test runs.  Will wait for a timeout
        /// if the expectation is not initially true.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="msWaitMax">The time to wait, in milliseconds.</param>
        /// <param name="msWaitIncrement">The polling interval, in milliseconds.</param>
        public void expectEventually(Expression<Func<bool>> expectation, int? msWaitMax = null, int? msWaitIncrement = null)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitWaitUntil(SpecElement.expectEventually, expectation, msWaitMax ?? _msWaitMax, msWaitIncrement ?? _msWaitIncrement, context.Position);
            }
        }

        /// <summary>
        /// Verifies a particular expectation is true as the test runs.  Will wait for a timeout
        /// if the expectation is not initially true.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="msWaitMax">The time to wait, in milliseconds.</param>
        /// <param name="msWaitIncrement">The polling interval, in milliseconds.</param>
        public void waitUntil(Expression<Func<bool>> expectation, int? msWaitMax = null, int? msWaitIncrement = null)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitWaitUntil(SpecElement.waitUntil, expectation, msWaitMax ?? _msWaitMax, msWaitIncrement ?? _msWaitIncrement, context.Position);
            }
        }

        public void withCategory(string category)
        {
            using (var context = SetPositionForNestedReentry_then_Restore_and_Advance_for_Next())
            {
                base.Visitor.visitWithCategory(SpecElement.withCategory, category, context.Position);
            }
        }
    }
}