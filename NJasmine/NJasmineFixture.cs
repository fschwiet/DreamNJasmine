using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Should.Fluent;
using Should.Fluent.Model;

namespace NJasmine
{
    public abstract class NJasmineFixture
    {
        public abstract void RootDescribe(Action action);

        public void describe(string description, Action action)
        {
        }

        public void beforeEach(Action action)
        {
        }

        public void forEach<T>(Func<IEnumerable<T>> testCases, Action<T> action)
        {
        }

        public void afterEach(Action action)
        {
        }

        public void it(string description, Action action)
        {
        }

        public NegateableExpectActual<T> expect<T>(T t)
        {
            return new NegateableExpectActual<T>() { toBe = t.Should<T, T>().Be, not = new ExpectActual<T>() { toBe = t.Should<T,T>().Not.Be } };
        }

        public class ExpectActual<T>
        {
            public Be<T> toBe;
        }

        public class NegateableExpectActual<T> : ExpectActual<T>
        {
            public ExpectActual<T> not;
        }
    }
}
