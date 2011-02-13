using System;
using System.Linq.Expressions;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine
{
    public abstract class GivenWhenThenFixture : SkeleFixture
    {
        public void given(string givenPhrase, Action specification)
        {
            SpecVisitor.visitFork(SpecElement.given, "given " + givenPhrase, specification);
        }

        public void when(string whenPhrase, Action specification)
        {
            SpecVisitor.visitFork(SpecElement.when, "when " + whenPhrase, specification);
        }

        public void then(string thenPhrase, Action test)
        {
            SpecVisitor.visitTest(SpecElement.then, "then " + thenPhrase, test);
        }

        public void then(string thenPhrase)
        {
            SpecVisitor.visitTest(SpecElement.then, "then " + thenPhrase, null);
        }

        public void cleanup(Action cleanup)
        {
            SpecVisitor.visitAfterEach(SpecElement.cleanup, cleanup);
        }

        public void arrange(Action arrangeAction)
        {
            SpecVisitor.visitBeforeEach(SpecElement.arrange, null, delegate() { arrangeAction(); return (string)null; });
        }

        public T arrange<T>(Func<T> arrangeAction)
        {
            return SpecVisitor.visitBeforeEach(SpecElement.arrange, null, arrangeAction);
        }

        public void expect(Expression<Func<bool>> expectation)
        {
            PowerAssert.PAssert.IsTrue(expectation);
        }

        public void beforeAll(Action action)
        {
            SpecVisitor.visitBeforeAll<string>(SpecElement.beforeAll, delegate {
                action();
                return (string)null;
            });
        }

        public T beforeAll<T>(Func<T> action)
        {
            return SpecVisitor.visitBeforeAll(SpecElement.beforeAll, action);
        }

        public void AfterAll(Action action)
        {
            SpecVisitor.visitAfterAll(SpecElement.afterAll, action);
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return SpecVisitor.visitImportNUnit<TFixture>();
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