using System;
using System.Linq.Expressions;
using System.Threading;
using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine
{
    public abstract class SpecificationFixture 
    {
        internal TestPosition CurrentPosition { get; set; }
        internal ISpecPositionVisitor Visitor { get; set; }

        public SpecificationFixture()
        {
        }

        public void Run()
        {
            this.Specify();
        }

        public abstract void Specify();

        public void expect(Expression<Func<bool>> expectation)
        {
            PowerAssert.PAssert.IsTrue(expectation);
        }

        private int _totalWaitMs = 1000;
        private int _incrementMs = 250;

        public void setWaitTimeouts(int totalWaitMs, int incrementMs)
        {
            _totalWaitMs = totalWaitMs;
            _incrementMs = Math.Min(incrementMs, 1);
        }

        public void waitUntil(Expression<Func<bool>> expectation, int? msMaxWait = null)
        {
            var expectationChecker = expectation.Compile();

            DateTime finishTime = DateTime.UtcNow.AddMilliseconds(msMaxWait ?? _totalWaitMs);

            while (!(expectationChecker()) && DateTime.Now < finishTime)
            {
                Thread.Sleep(_incrementMs);
            }

            PowerAssert.PAssert.IsTrue(expectation);
        }
    }
}