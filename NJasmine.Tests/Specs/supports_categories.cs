using System;
using NJasmine;

namespace NJasmineTests.Specs
{
    [RunExternal(true, VerificationScript = @"

param ($consoleOutput, $xmlFile);

import-module .\tools\PSUpdateXML.psm1

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

    assertShouldHaveCategories 'test-case' 'NJasmineTests.Specs.supports_categories, when using category Foo then Bar, then tests have Foo' @(""Foo"")

    assertShouldHaveCategories 'test-case' 'NJasmineTests.Specs.supports_categories, when using category Foo then Bar, then tests have For and Bar' @('Foo', 'Bar')

    assertShouldHaveCategories 'test-suite' 'when in a nested block and using a category' @('Foo', 'Bar')

    assertShouldHaveCategories 'test-case' 'NJasmineTests.Specs.supports_categories, when using category Foo then Bar, when in a nested block and using a category, then the test only has category Baz' @('Baz')
}
")]
    public class supports_categories : GivenWhenThenFixture
    {
        public class Categories
        {
            public static readonly string Foo = "Foo";
            public static readonly string Bar = "Bar";
            public static readonly string Baz = "Baz";
        }

        public override void Specify()
        {
            when("using category Foo then Bar", delegate
            {
                withCategory(Categories.Foo);

                then("tests have Foo", delegate { });

                withCategory(Categories.Bar);

                then("tests have For and Bar", delegate { });

                when("in a nested block and using a category", delegate
                {
                    withCategory(Categories.Baz);

                    then("the nested block has category Foo, Bar", delegate { });

                    then("the test only has category Baz", delegate { });
                });
            });
        }
    }
}
