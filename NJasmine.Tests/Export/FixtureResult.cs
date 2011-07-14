using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmineTests.Export
{
    public class FixtureResult
    {
        private readonly bool _passed;
        private readonly string _output;
        private readonly string _xmlOutput;

        public FixtureResult(bool passed, string output, string xmlOutput)
        {
            _passed = passed;
            _output = output;
            _xmlOutput = xmlOutput;
        }

        public TestResult hasTest(string name)
        {
            // should assert the test actually exists...
            throw new NotImplementedException();
        }

        public void succeeds()
        {
            throw new NotImplementedException();
        }

        public void failed()
        {
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
    }
}
