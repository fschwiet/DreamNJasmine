//using Should.Fluent;
using System;
using Should.Fluent.Model;



namespace NJasmine
{
    public abstract class ExpectationsFixture
    {
        protected NegateableExpectActual<T> expect<T>(T t)
        {
            return new NegateableExpectActual<T>()
            {
                to = Should.Fluent.ShouldExtensions.Should<T, T>(t),
            };
        }

        protected NegateableExpectActual<Action> expect(Action t)
        {
            return new NegateableExpectActual<Action>()
            {
                to = Should.Fluent.ShouldExtensions.Should<Action, Action>(t),
            };
        }

        protected class ExpectActual<T>
        {
            public Should<T, Be<T>> to;
        }

        protected class NegateableExpectActual<T> : ExpectActual<T>
        {
            public ExpectActual<T> not 
            {
                get { return new ExpectActual<T>() {to = this.to.Not}; }
            }
        }
    }
}