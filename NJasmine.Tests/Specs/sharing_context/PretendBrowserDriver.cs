using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmineTests.Specs.sharing_context
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
