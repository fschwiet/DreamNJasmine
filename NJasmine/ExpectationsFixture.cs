//using Should.Fluent;
using Should.Fluent.Model;



namespace NJasmine
{
    public abstract class ExpectationsFixture
    {
        protected NegateableExpectActual<T> expect<T>(T t)
        {
            Should<T, Be<T>> t2 = Should.Fluent.ShouldExtensions.Should<T,T>(t);

            return new NegateableExpectActual<T>() { 
                to = t2,
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