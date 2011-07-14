using System;

namespace NJasmineTests.Export
{
    public class TestResult
    {
        public TestResult withMessage(string message)
        {
            throw new NotImplementedException();
        }

        public TestResult thatSucceeds()
        {
            // $null = get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.Specs.beforeAll.beforeAll_can_use_expectations, works'][@result='Success']"" 
            throw new NotImplementedException();
        }

        public TestResult thatErrors()
        {
            // $null = get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.Specs.beforeAll.beforeAll_can_use_expectations, when using expect within beforeAll, fails'][@result='Error']"" 
            throw new NotImplementedException();
        }

        public TestResult thatFailsInAnUnspecifiedManner()
        {
            throw new NotImplementedException();
        }

        public TestResult thatIsNotRunnable()
        {
            // console output:  "NotRunnable : NJasmineTests.Specs.duplicate_test_names_are_fine, repeated unimplemented outer test",
            throw new NotImplementedException();
        }

        public TestResult thatFails()
        {
            // console output: "Test Failure : NJasmineTests.Specs.duplicate_test_names_are_fine, repeated describe, repeated inner describe",
            throw new NotImplementedException();
        }

        public TestResult withCategories(params string[] categories)
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
}