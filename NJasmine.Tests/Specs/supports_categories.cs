using System;
using NJasmine;
using NJasmine.Marshalled;
using NJasmineTests.Export;

namespace NJasmineTests.Specs
{
    public class supports_categories : GivenWhenThenFixture, INJasmineInternalRequirement
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

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.succeeds();

            var suite = fixtureResult.hasSuite("when using category Foo then Bar").withCategories();

            suite.hasTest("then tests have Foo", t => t.withCategories("Foo"));

            suite.hasTest("then tests have Foo", t => t.withCategories("Foo"));

            suite.hasTest("then tests have For and Bar", t => t.withCategories("Foo", "Bar"));

            var nestedSuite = suite.hasSuite("when in a nested block and using a category").withCategories("Foo", "Bar");

            nestedSuite.hasTest("then the test only has category Baz", t => t.withCategories("Baz"));
        }
    }
}
