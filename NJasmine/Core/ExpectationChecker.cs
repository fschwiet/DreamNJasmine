using System;
using System.Linq.Expressions;
using System.Threading;

namespace NJasmine.Core
{
    class ExpectationChecker
    {
        public static void Expect(Expression<Func<bool>> expectation)
        {
            PowerAssert.PAssert.IsTrue(expectation);
        }

        public static void WaitUntil(Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs)
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
    }
}