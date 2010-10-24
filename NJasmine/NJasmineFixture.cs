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
        public abstract void BuildTests(Action action);

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

        public ExpectActual<T> expect<T>(T t)
        {
            return new ExpectActual<T>() { to = t.Should<T, T>().Be };
        }

        public class ExpectActual<T>
        {
            public Be<T> to;
        }
    }
}
