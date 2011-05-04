using System;
using System.Linq.Expressions;

namespace NJasmineTests.Core
{
    public class PowerAssertFixture
    {
        public void expect(Expression<Func<bool>> expectation)
        {
            PowerAssert.PAssert.IsTrue(expectation);
        }
    }
}