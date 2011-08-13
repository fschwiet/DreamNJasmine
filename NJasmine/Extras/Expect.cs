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
    }
}