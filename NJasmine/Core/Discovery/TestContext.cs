using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core.Discovery
{
    public class TestContext
    {
        public TestName Name;
        public TestPosition Position;
        public IGlobalSetupManager GlobalSetupManager;
    }
}
