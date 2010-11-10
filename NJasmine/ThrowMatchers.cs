using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Should.Fluent.Model;

namespace NJasmine
{
    public static class ThrowMatchers
    {
        public static void Throw<TException>(this IShould<Action> should) where TException : Exception
        {
            should.Apply((actual, assert) =>
                {
                    Exception e = null;

                    try
                    {
                        actual();
                    }
                    catch (Exception thrown)
                    {
                        e = thrown;
                    }

                    if (e == null)
                    {
                        assert.Fail("Expected exception of type {0}, no exception was thrown.", typeof(TException).Name);
                    }
                    else if (!typeof(TException).IsInstanceOfType(e))
                    {
                        assert.Fail("Expected exception of type {0}, saw exception of type {1}.", typeof(TException).Name, e.GetType().Name);
                    }

                    return actual;
                },
                (actual, assert) =>
                {
                    Exception e = null;

                    try
                    {
                        actual();
                    }
                    catch (Exception thrown)
                    {
                        e = thrown;
                    }

                    if (typeof(TException).IsInstanceOfType(e))
                    {
                        assert.Fail("Expected not to see exception of type {0}, saw exception of type {1}.", typeof(TException).Name, e.GetType().Name);
                    }

                    return actual;
                });
        }
    }
}
