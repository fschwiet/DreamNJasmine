using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class SuiteResult
    {
        private readonly string _name;
        private readonly XElement _xml;

        public SuiteResult(XElement xml)
        {
            _xml = xml;

            if (xml.Attribute("name") != null)
                _name = xml.Attribute("name").Value;
            else
                _name = "unknown";
        }

        public SuiteResult thatsInconclusive()
        {
            return thatHasResult("Inconclusive");
        }

        public SuiteResult thatSucceeds()
        {
            return thatHasResult("Success");
        }

        private SuiteResult thatHasResult(string inconclusive)
        {
            var result = _xml.Attribute("result").Value;

            Assert.AreEqual(inconclusive, result, String.Format("Expected suite named {0} to be {1}.", _name, inconclusive));

            return this;
        }

        public SuiteResult thatHasNoResults()
        {
            var results = _xml.Descendants("results");

            Assert.AreEqual(0, results.Count(), "Expected suite " + _name + " to not have any results.");

            return this;
        }

        public TestResult ShouldHaveTest(string testName)
        {
            throw new NotImplementedException();
        }

        public void withCategories(params string[] categories)
        {
            /*
                                 function assertShouldHaveCategories($typeName, $name, $expectedCategories) {

                    $nonempty = $expectedCategories.length -gt 0

                    for-xml -exactlyOnce:$nonempty ""//$typeName[@name='$name']/categories"" {

                        foreach($expectedCategory in $expectedCategories) {
                            Assert ((get-xml ""category[@name='$expectedCategory']"") -ne  $null) `
                                ""Expected '$test' to have category $expectedCategory""
                        }

                        for-xml ""category"" {

                            $otherCategoryName = get-xml ""@name""
                            Assert ($expectedCategories -contains $otherCategoryName) ""Expected '$test' to NOT have category $otherCategoryName""
                        }
                    }
                }

                assertShouldHaveCategories 'test-suite' 'when using category Foo then Bar' @()

                assertShouldHaveCategories 'test-case' 'NJasmineTests.Specs.supports_categories, when using category Foo then Bar, then tests have Foo' @(""Foo"")* 
                 */

            throw new NotImplementedException();
        }
    }
}