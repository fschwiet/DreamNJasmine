using System;
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
            var result = _xml.Attribute("result").Value;

            Assert.AreEqual("Inconclusive", result, "Expected suite named " + _name + " to be Inconclusive.");

            return this;
        }

        public SuiteResult thatHasNoResults()
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

        public SuiteResult thatSucceeds()
        {
            /*
                for-xml -exactlyOnce ""//test-suite[@name='when ignore is set after a test']"" {
                    Assert ((get-xml ""@result"") -eq 'Success') 'Expected when statement with non-ignored test to have succeeded.';

                    for-xml -exactlyOnce ""./results/test-case"" {
                        Assert ((get-xml ""@name"") -eq 'NJasmineTests.Specs.can_mark_tests_as_ignored, when ignore is set after a test, then the earlier test runs') `
                            'Expected when statement with non-ignored test to contain the non-ignored test'

                        Assert ((get-xml ""@result"") -eq 'Success') 'Expected non-ignored test to have passed';
                    }
                }
                */

            throw new NotImplementedException();
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