using System;
using System.Linq.Expressions;

namespace NJasmine
{
    public interface IGivenWhenThenContext
    {
        ISpecVisitor SpecVisitor { get; }
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
}