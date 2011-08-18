using System;
using System.Linq.Expressions;
using System.Threading;

namespace NJasmine.Extras
{
    public class Expect
    {
        public static void That(Expression<Func<bool>> expectation)
        {
            PowerAssert.PAssert.IsTrue(expectation);
        }

        public static void Eventually(Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs)
        {
            var expectationChecker = expectation.Compile();

            DateTime finishTime = DateTime.UtcNow.AddMilliseconds(totalWaitMs);

            bool passing;

            while (!(passing = expectationChecker()) && DateTime.UtcNow < finishTime)
            {
                Thread.Sleep(waitIncrementMs);
            }

            if (!passing)
                PowerAssert.PAssert.IsTrue(expectation);
        }

        public class StringExpection
        {
            private readonly string _value;

            public StringExpection(string value)
            {
                _value = value;
            }

            public void ContainsInOrder(params string[] expectedSubstrings)
            {
                int lastPosition = 0;

                foreach(var substring in expectedSubstrings)
                {
                    Expect.That(() => _value.IndexOf(substring, lastPosition) > -1);
                    lastPosition = _value.IndexOf(substring) + 1;
                }
            }
        }

        public static StringExpection That(string value)
        {
            return new StringExpection(value);
        }
    }
}