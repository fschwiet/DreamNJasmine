using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmineTests.Export
{
    public class VS2012FixtureResult : IFixtureResult
    {
        public VS2012FixtureResult(string testName, string trvContents, string consoleContents)
        {
        }

        public IFixtureResult succeeds()
        {
            return this;
        }

        public IFixtureResult failed()
        {
            return this;
        }

        public void containsTrace(string expectedTrace)
        {
        }

        public ITestResult hasTest(string name)
        {
            return new VS2012TestResult();
        }

        public ITestResult hasTestWithFullName(string name)
        {
            return new VS2012TestResult();
        }

        public ISuiteResult hasSuite(string name)
        {
            return new VS2012SuiteResult();
        }

        public string[] withStackTraces()
        {
            return new string[0];
        }
    }
}
