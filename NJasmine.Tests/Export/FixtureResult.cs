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

        public TestDescription hasTest(string name)
        {
            // should assert the test actually exists...
            throw new NotImplementedException();
        }

        public class TestDescription
        {
            public TestDescription withMessage(string message)
            {
                throw new NotImplementedException();
            }

            public TestDescription thatSucceeds()
            {
                // $null = get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.Specs.beforeAll.beforeAll_can_use_expectations, works'][@result='Success']"" 
                throw new NotImplementedException();
            }

            public TestDescription thatErrors()
            {
                // $null = get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.Specs.beforeAll.beforeAll_can_use_expectations, when using expect within beforeAll, fails'][@result='Error']"" 
                throw new NotImplementedException();
            }

            public TestDescription thatFailsInAnUnspecifiedManner()
            {
                throw new NotImplementedException();
            }

            public TestDescription thatIsNotRunnable()
            {
                // console output:  "NotRunnable : NJasmineTests.Specs.duplicate_test_names_are_fine, repeated unimplemented outer test",
                throw new NotImplementedException();
            }

            public TestDescription thatFails()
            {
                // console output: "Test Failure : NJasmineTests.Specs.duplicate_test_names_are_fine, repeated describe, repeated inner describe",
                throw new NotImplementedException();
            }

            public TestDescription withCategories(params string[] categories)
            {
                /*
                    update-xml $xmlFile {

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

                        assertShouldHaveCategories 'test-case' 'NJasmineTests.Specs.supports_categories, when using category Foo then Bar, then tests have Foo' @(""Foo"")                 */

                throw new NotImplementedException();
            }
        }

        public class SuiteDescription
        {
            public SuiteDescription thatsInconclusive()
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

            public SuiteDescription thatHasNoResults()
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

            public SuiteDescription thatSucceeds()
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

            public SuiteDescription thatHasOneResult()
            {
                throw new NotImplementedException();
            }

            public TestDescription ShouldHaveTest(string testName)
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

        public SuiteDescription hasSuite(string name)
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
