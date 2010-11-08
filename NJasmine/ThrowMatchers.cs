using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Should.Fluent.Model;
using Should.Fluent.Extensions;

namespace NJasmine
{
    public static class ThrowMatchers
    {
        public static void Throw<TException>(this Should<Action, Be<Action>> should) where TException : Exception
        {
            Exception e = null;

            try
            {
                should.Actual()();
            }
            catch (Exception thrown)
            {
                e = thrown;
            }

            if (should.ShouldNegate())
            {
                if (typeof(TException).IsInstanceOfType(e))
                {
                    should.Assert().Fail("Expected not to see exception of type {0}, saw exception of type {1}.", typeof(TException).Name, e.GetType().Name);
                }
            }
            else
            {
                if (e == null)
                {
                    should.Assert().Fail("Expected exception of type {0}, no exception was thrown.", typeof(TException).Name);
                }
                else if (!typeof(TException).IsInstanceOfType(e))
                {
                    should.Assert().Fail("Expected exception of type {0}, saw exception of type {1}.", typeof(TException).Name, e.GetType().Name);
                }
            }
        }
    }
}
