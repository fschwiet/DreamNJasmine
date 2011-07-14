using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class FixtureResult
    {
        private readonly string _name;
        private readonly string _xmlOutput;
        private XDocument _doc;

        public FixtureResult(string name, string xmlOutput)
        {
            _name = name;
            _xmlOutput = xmlOutput;
            _doc = XDocument.Parse(xmlOutput);
        }

        public FixtureResult succeeds()
        {
            int totalCount = (int)_doc.Root.Attribute("total");
            int errorCount = GetErrorCount();
            int failureCount = GetFailureCount();

            Assert.AreEqual(0, errorCount, _name + " had errors.");
            Assert.AreEqual(0, failureCount, _name + " had failures.");
            Assert.AreNotEqual(0, totalCount, _name + " didn't have any tests.");

            return this;
        }

        public FixtureResult failed()
        {
            Assert.AreNotEqual(0, GetErrorCount() + GetFailureCount(), _name + " didn't have errors.");

            return this;
        }

        public TestResult hasTest(string name)
        {
            // should assert the test actually exists...
            throw new NotImplementedException();
        }

        public void containsTrace(string s)
        {
            throw new NotImplementedException();
        }

        public SuiteResult hasSuite(string name)
        {
            /*
            for-xml -exactlyOnce ""//test-suite[@name='given an outer block']"" {
                Assert ((get-xml ""@result"") -eq 'Inconclusive') 'Expected first outer block to be inconclusive';

                for-xml ""./results"" {
                    Assert $false 'Expected first outer block to not have any subresults'
                }
            }
            */

            throw new NotImplementedException();
        }

        public void hasStackTracesThat(Func<string, bool> expectation)
        {
            /*
                update-xml $xmlFile {                    $stackTrace = get-xml -exactlyonce ""//stack-trace"";                    assert (-not ($stackTrace -like '*NJasmine.Core*')) 'Expected NJasmine.Core stack elements to be removed';            */
            throw new NotImplementedException();
        }

        private int GetErrorCount()
        {
            return (int)_doc.Root.Attribute("errors");
        }

        private int GetFailureCount()
        {
            return (int)_doc.Root.Attribute("failures");
        }
    }
}
