using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NJasmineTests.Export
{
    public class VS2012FixtureResult : IFixtureResult
    {
        public XDocument _trxResults;

        public VS2012FixtureResult(string trxXmlContents)
        {
            _trxResults = XDocument.Parse(trxXmlContents);
        }

        public IFixtureResult succeeds()
        {
            var counts = GetResultSummaryCounts();

            if (counts.Executed == 0)
                throw new Exception("Expected fixture result to be succeeded, but no tests were executed.");

            if (counts.Passed < counts.Executed)
                throw new Exception("Expected fixture result to be succeeded, but not all executed tests passed.");

            return this;
        }

        public IFixtureResult failed()
        {
            var counts = GetResultSummaryCounts();

            if (counts.Executed == 0)
                throw new Exception("Expected fixture result to fail, but no tests were executed.");

            if (counts.Executed == counts.Passed)
                throw new Exception("Expected fixture result to fail, but all executed tests passed.");

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

        class ResultSummaryCounts
        {
            public int Executed;
            public int Passed;
        }

        private ResultSummaryCounts GetResultSummaryCounts()
        {
            var resultSummary = _trxResults.Descendants("ResultSummary").Single().Descendants("Counters").Single();

            var counts = new ResultSummaryCounts()
            {
                Executed = int.Parse(resultSummary.Attribute("executed").Value),
                Passed = int.Parse(resultSummary.Attribute("passed").Value),
            };
            return counts;
        }
    }
}
