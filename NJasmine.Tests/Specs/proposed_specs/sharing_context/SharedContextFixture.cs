using NJasmine;

namespace NJasmineTests.Specs.proposed_specs.sharing_context
{
    class SharedContextFixture : SharedFixture
    {
        public IWebDriver WebDriver;

        public override void Specify()
        {
            given("a web browser", delegate()
            {
                WebDriver = beforeAll(() => new FakeDriver());

                run();
            });
        }
    }
}
