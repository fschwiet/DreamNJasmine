using System;
using NJasmine;

namespace NJasmineTests.Specs
{
    [RunExternal(true, VerificationScript = @"

param ($consoleOutput, $xmlFile);

import-module .\lib\PSUpdateXML\PSUpdateXML.psm1

update-xml $xmlFile {

    Assert (""//test-suite[@name='when using category Foo']/categories"" -eq $null) `
        ""The category shouldn't be assigned to the containing block""

    Assert (""//test-suite[@name='when using category Foo, then tests have that category']/categories/category/@name"" -eq ""Foo"") `
        ""test should have category Foo""

    $expectedFooBar = ""Foo+Bar""

    Assert (""//test-suite[@name='when using category Foo, then tests have both categories']/categories/category/@name"" -eq $expectedFooBar) `
        ""test should have category Foo+Bar""

    Assert (""//test-suite[@name='when using category Foo, when in a nested block and using a category']/categories/category/@name"" -eq $expectedFooBar) `
        ""discovery block should have category Foo+Bar""

    Assert (""//test-suite[@name='when using category Foo, when in a nested block and using a category, then the test only has category Baz']/categories/category/@name"" -eq ""Baz"") `
        ""Test should have category Baz""
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

                then("tests have Foo");

                withCategory(Categories.Foo);

                then("tests have For and Bar");

                when("in a nested block and using a category", delegate
                {
                    withCategory(Categories.Baz);

                    then("the nested block has category Foo, Bar");

                    then("the test only has category Baz");
                });
            });
        }
    }
}
