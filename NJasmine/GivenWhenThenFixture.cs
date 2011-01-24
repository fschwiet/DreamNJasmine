using System;
using System.Linq.Expressions;

namespace NJasmine
{
    public interface ITestExecutionVisitor
    {
        void cleanup(Action cleanup);
        void arrange(Action arrangeAction);
        T arrange<T>(Func<T> arrangeAction);
        void expect(Expression<Func<bool>> expectation);
    }

    public interface ISpecificationVisitor
    {
        void given(string givenPhrase, Action specification);
        void when(string whenPhrase, Action specification);
        void then(string thenPhrase, Action test);
        void then(string thenPhrase);
        void cleanup(Action cleanup);
        void arrange(Action arrangeAction);
        T arrange<T>(Func<T> arrangeAction);
        void expect(Expression<Func<bool>> expectation);
        TFixture importNUnit<TFixture>() where TFixture : class, new();
    }

    public abstract class GivenWhenThenFixture : SkeleFixture, ISpecificationVisitor, ITestExecutionVisitor
    {
        public NJasmineFixture _internal = new InnerFixture();

        public override NJasmineFixture.VisitorChangedContext UseVisitor(Core.FixtureVisitor.INJasmineFixtureVisitor visitor)
        {
            return _internal.UseVisitor(visitor);
        }

        public void given(string givenPhrase, Action specification)
        {
            _internal.describe("given " + givenPhrase, specification);
        }

        public void when(string whenPhrase, Action specification)
        {
            _internal.describe("when " + whenPhrase, specification);
        }

        public void then(string thenPhrase, Action test)
        {
            _internal.it("then " + thenPhrase, test);
        }

        public void then(string thenPhrase)
        {
            _internal.it("then " + thenPhrase);
        }

        public void cleanup(Action cleanup)
        {
            _internal.afterEach(cleanup);
        }

        public void arrange(Action arrangeAction)
        {
            _internal.arrange(arrangeAction);
        }

        public T arrange<T>(Func<T> arrangeAction)
        {
            return _internal.arrange(arrangeAction);
        }

        public void expect(Expression<Func<bool>> expectation)
        {
            PowerAssert.PAssert.IsTrue(expectation);
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return _internal.importNUnit<TFixture>();
        }

        public class InnerFixture : NJasmineFixture
        {
            public override void Specify()
            {
                throw new Exception("this Specify method won't be used");
            }
        }
    }
}