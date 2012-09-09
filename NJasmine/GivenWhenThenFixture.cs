using System;
using System.Linq.Expressions;
using System.Threading;
using NJasmine.Core;
using NJasmine.Core.Elements;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    public abstract class GivenWhenThenFixture : SpecificationFixture
    {
        /// <summary>
        /// Branches the current test specification
        /// </summary>
        /// <param name="description">Description text used to name the test branch.</param>
        /// <param name="specification">The branched portion of the specification.</param>
        public void describe(string description, Action specification)
        {
            RunSpecificationElement(new ForkElement(ActualKeyword.describe, description, specification));
        }

        /// <summary>
        /// Branches the current test specification
        /// </summary>
        /// <param name="description">Description text used to name the test branch -- will be prefixed with 'given'.</param>
        /// <param name="specification">The branched portion of the specification.</param>
        public void given(string description, Action specification)
        {
            RunSpecificationElement(new ForkElement(ActualKeyword.describe, "given " + description, specification));
        }

        /// <summary>
        /// Branches the current test specification.
        /// </summary>
        /// <param name="description">Description text used to name the test branch -- will be prefixed with 'when'.</param>
        /// <param name="specification">The branched portion of the specification.</param>
        public void when(string description, Action specification)
        {
            RunSpecificationElement(new ForkElement(ActualKeyword.describe, "when " + description, specification));
        }

        /// <summary>
        /// Adds a test.
        /// </summary>
        /// <param name="description">The description that names the test -- will be prefixed with 'then'.</param>
        /// <param name="test">The test implementation.</param>
        public void then(string description, Action test)
        {
            RunSpecificationElement(new TestElement(ActualKeyword.then, "then " + description, test));
        }

        /// <summary>
        /// Adds an unimplemented test.
        /// </summary>
        /// <param name="description">The description that names the test -- will be prefixed with 'then'.</param>
        public void then(string description)
        {
            RunSpecificationElement(new TestElement(ActualKeyword.then, "then " + description, null));
        }

        /// <summary>
        /// Adds a test
        /// </summary>
        /// <param name="description">The description that names the test.</param>
        /// <param name="action">The test implementation.</param>
        public void it(string description, Action action)
        {
            RunSpecificationElement(new TestElement(ActualKeyword.it, description, action));
        }

        /// <summary>
        /// Adds an unimplemented test.
        /// </summary>
        /// <param name="description">The description that names the test.</param>
        public void it(string description)
        {
            RunSpecificationElement(new TestElement(ActualKeyword.it, description, null));
        }

        /// <summary>
        /// Adds cleanup code to be ran after each test in the following context.
        /// </summary>
        /// <param name="action">The cleanup code.</param>
        public void afterEach(Action action)
        {
            RunSpecificationElement(new AfterEachElement(ActualKeyword.afterEach, action));
        }

        /// <summary>
        /// Adds cleanup code to be ran after each test in the following context.
        /// </summary>
        /// <param name="action">The cleanup code.</param>
        public void cleanup(Action action)
        {
            RunSpecificationElement(new AfterEachElement(ActualKeyword.cleanup, action));
        }

        /// <summary>
        /// Adds initialization code to be before after each test in the following context.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        public void beforeEach(Action action)
        {
            RunSpecificationElement(new BeforeEachElementWithoutReturnValue(ActualKeyword.beforeEach, action));
        }

        /// <summary>
        /// Adds initialization code to be before after each test in the following context.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        public T beforeEach<T>(Func<T> action)
        {
            return RunSpecificationElement<T>(new BeforeEachElement<T>(ActualKeyword.beforeEach, action));
        }

        /// <summary>
        /// Adds initialization code to be before after each test in the following context.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        public void arrange(Action action)
        {
            RunSpecificationElement(new BeforeEachElementWithoutReturnValue(ActualKeyword.arrange, action));
        }

        /// <summary>
        /// Adds initialization code to be before after each test in the following context.
        /// A return value can be used in the remainder of the test.
        /// If that return value supports IDisposable, it will be disposed when the test is done.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        /// <returns>The result of the initialization code.</returns>
        public T arrange<T>(Func<T> action)
        {
            return RunSpecificationElement<T>(new BeforeEachElement<T>(ActualKeyword.arrange, action));
        }

        public T with<T>(Action<T> action) where T : SharedFixture, new()
        {
            return RunSpecificationElement<T>(new WithElement<T>(ActualKeyword.with, () => new T(), action));
        }
        /// <summary>
        /// Adds initialization code to be ran once before all tests in the following context.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        public void beforeAll(Action action)
        {
            RunSpecificationElement(new BeforeAllElementWithoutReturnValue(ActualKeyword.beforeAll, action));
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
            return RunSpecificationElement<T>(new BeforeAllElement<T>(ActualKeyword.beforeAll, action));
        }

        /// <summary>
        /// Adds cleanup code to be ran once after all tests in the following context.
        /// </summary>
        /// <param name="action">The cleanup code.</param>
        public void afterAll(Action action)
        {
            RunSpecificationElement(new AfterAllElement(ActualKeyword.afterAll, action));
        }

        /// <summary>
        /// Indicates that the tests in the following specification context should not
        /// be ran unless the user explicitly asks for them to be ran.  Similar to
        /// NUnit's ExplicitAttribute.
        /// </summary>
        /// <param name="reason"></param>
        public void ignoreBecause(string reason)
        {
            RunSpecificationElement(new IgnoreElement(ActualKeyword.ignore, reason));
        }

        /// <summary>
        /// Verifies a particular expecation when the tests run.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        public void expect(Expression<Func<bool>> expectation)
        {
            RunSpecificationElement(new ExpectElement(ActualKeyword.expect, expectation));
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
            RunSpecificationElement(new WaitUntilElement(ActualKeyword.expectEventually, expectation, msWaitMax ?? _msWaitMax, msWaitIncrement ?? _msWaitIncrement));
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
            RunSpecificationElement(new WaitUntilElement(ActualKeyword.waitUntil, expectation, msWaitMax ?? _msWaitMax, msWaitIncrement ?? _msWaitIncrement));
        }

        public void withCategory(string category)
        {
            RunSpecificationElement(new WithCategoryElement(ActualKeyword.withCategory, category));
        }

        public void trace(string message)
        {
            RunSpecificationElement(new TraceElement(ActualKeyword.trace,  message));
        }

        public void leakDisposable(IDisposable disposable)
        {
            RunSpecificationElement(new LeakDisposableElement(ActualKeyword.leakDisposable, disposable));
        }
    }
}