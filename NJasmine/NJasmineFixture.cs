using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
