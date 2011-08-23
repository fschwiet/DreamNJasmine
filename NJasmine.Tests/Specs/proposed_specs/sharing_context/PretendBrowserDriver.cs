namespace NJasmineTests.Specs.proposed_specs.sharing_context
{
    public interface IWebDriver
    {
        void Goto(string url);
        string GetText();
    }

    public class FakeDriver : IWebDriver
    {
        public void Goto(string url)
        {
        }

        public string GetText()
        {
            return null;
        }
    }

    public class FirefoxBrowser : FakeDriver { }
    public class ChromeBrowser : FakeDriver { }
    public class InternetExplorerBroser : FakeDriver { }
}
