namespace NJasmineTests.Specs.proposed_specs.sharing_context
{
    class SharedContextFixture : SharedFixtureBase
    {
        public IWebDriver WebDriver;

        public override void Specify()
        {
            given("a web browser", delegate()
            {
                WebDriver = beforeAll(() => new FakeDriver());

                becomesContext("with browser");
            });
        }
    }
}
