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

        private int _totalWaitMs;
        private int _incrementMs;

        public void setWaitTimeouts(int totalWaitMs, int incrementMs)
        {
            _totalWaitMs = totalWaitMs;
            _incrementMs = incrementMs;
        }

        public void waitUntil(Expression<Func<bool>> expectation, int? msMaxWait = null)
        {
            var expectationChecker = expectation.Compile();

            int waitLeft = msMaxWait ?? _totalWaitMs;

            while (!(expectationChecker()) && waitLeft > 0)
            {
                Thread.Sleep(_incrementMs);
                waitLeft -= _incrementMs;
            }

            PowerAssert.PAssert.IsTrue(expectation);
        }
    }
}