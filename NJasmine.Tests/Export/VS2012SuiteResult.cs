using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmineTests.Export
{
    public class VS2012SuiteResult : ISuiteResult
    {
        public ISuiteResult thatsInconclusive()
        {
            return this;
        }

        public ISuiteResult thatSucceeds()
        {
            return this;
        }

        public ISuiteResult thatHasNoResults()
        {
            return this;
        }

        public ISuiteResult hasTest(string expectedName, Action<ITestResult> handler)
        {
            return this;
        }

        public ISuiteResult withCategories(params string[] categories)
        {
            return this;
        }

        public ISuiteResult hasSuite(string name)
        {
            return this;
        }

        public ISuiteResult doesNotHaveTestContaining(string skipped)
        {
            return this;
        }
    }
}
