using System;
using System.Linq.Expressions;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine
{
    public class GivenWhenThenContext : IGivenWhenThenContext
    {
        readonly ISpecVisitor _specVisitor;

        public GivenWhenThenContext(ISpecVisitor specVisitor)
        {
            _specVisitor = specVisitor;
        }

        public void given(string givenPhrase, Action specification)
        {
            _specVisitor.visitBeforeEach(SpecElement.given, "given " + givenPhrase, delegate()
            {
                specification();
                return (string) null;
            });
        }

        public void when(string whenPhrase, Action specification)
        {
            _specVisitor.visitBeforeEach(SpecElement.when, "when " + whenPhrase, delegate()
            {
                specification();
                return (string)null;
            });
        }

        public void then(string thenPhrase, Action test)
        {
            _specVisitor.visitTest(SpecElement.then, "then " + thenPhrase, test);
        }

        public void then(string thenPhrase)
        {
            _specVisitor.visitTest(SpecElement.then, "then " + thenPhrase, null);
        }

        public void cleanup(Action cleanup)
        {
            _specVisitor.visitAfterEach(cleanup);
        }

        public void arrange(Action arrangeAction)
        {
            _specVisitor.visitBeforeEach(SpecElement.arrange, null, delegate
            {
                arrangeAction();
                return (string) null;
            });
        }

        public T arrange<T>(Func<T> arrangeAction)
        {
            return _specVisitor.visitBeforeEach(SpecElement.arrange, null, arrangeAction);
        }

        public void expect(Expression<Func<bool>> expectation)
        {
            PowerAssert.PAssert.IsTrue(expectation);
        }

        public TFixture importNUnit<TFixture>() where TFixture : class, new()
        {
            return _specVisitor.visitImportNUnit<TFixture>();
        }
    }
}