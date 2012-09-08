using NJasmineTests.Export;

namespace NJasmineTests.Specs.proposed_specs.sharing_context
{
    class HomepageTests : GivenWhenThenFixture2
    {
        public override void Specify()
        {
            var browser = includeContext<BrowserDriverContext>().Browser;

            arrange(() => browser.Goto("http://www.google.com/"));

            it("supports search", delegate
            {
                expect(() => browser.GetText().Contains("search"));
            });
        }

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.hasSuite("the browser is firefox").hasTest("it supports search", t => { });
            fixtureResult.hasSuite("the browser is internet explorer").hasTest("it supports search", t => { });
            fixtureResult.hasSuite("the browser is chrome").hasTest("it supports search", t => { });
        }
    }
}
