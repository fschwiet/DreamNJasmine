using System;
using System.Linq.Expressions;
using System.Threading;

namespace NJasmine
{
    public abstract class SpecificationFixture
    {
        protected SkeleFixture _skeleFixture;

        public SpecificationFixture()
        {
            _skeleFixture = new SkeleFixture(this.Specify);
        }

        protected SpecificationFixture(SkeleFixture fixture)
        {
            _skeleFixture = fixture;
        }

        public abstract void Specify();

        //  Making this static so people don't run across it generally
        public static SkeleFixture GetUnderlyingSkelefixture(SpecificationFixture fixture)
        {
            return fixture._skeleFixture;
        }

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

        public void waitUntil(Expression<Func<bool>> expectation)
        {
            var expectationChecker = expectation.Compile();

            int waitLeft = _totalWaitMs;

            while (!(expectationChecker()) && waitLeft > 0)
            {
                Thread.Sleep(_incrementMs);
                waitLeft -= _incrementMs;
            }

            PowerAssert.PAssert.IsTrue(expectation);
        }
    }
}