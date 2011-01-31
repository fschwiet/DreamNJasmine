using System;
using System.Linq.Expressions;
using NJasmine.Core.FixtureVisitor;

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
        public void given(string givenPhrase, Action specification)
        {
            _visitor.visitDescribe("given " + givenPhrase, specification);
        }

        public void when(string whenPhrase, Action specification)
        {
            _visitor.visitDescribe("when " + whenPhrase, specification);
        }

        public void then(string thenPhrase, Action test)
        {
            _visitor.visitIt("then " + thenPhrase, test);
        }

        public void then(string thenPhrase)
        {
            _visitor.visitIt("then " + thenPhrase, null);
        }

        public void cleanup(Action cleanup)
        {
            _visitor.visitAfterEach(cleanup);
        }

        public void arrange(Action arrangeAction)
        {
            _visitor.visitArrange(SpecMethod.arrange, null, new Func<string>[]
            {
                delegate() { arrangeAction(); return null; }
            });
        }

        public T arrange<T>(Func<T> arrangeAction)
        {
            return _visitor.visitArrange(SpecMethod.arrange, null, new Func<T>[] {arrangeAction});
        }

        public void expect(Expression<Func<bool>> expectation)
        {
            PowerAssert.PAssert.IsTrue(expectation);
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return _visitor.visitImportNUnit<TFixture>();
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