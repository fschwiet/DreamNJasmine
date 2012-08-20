using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core;
using NJasmine.Core.GlobalSetup;

namespace NJasmineTests.Core
{
    class FakeGlobalSetupManager : IGlobalSetupManager
    {
        public void Cleanup(TestPosition position)
        {
        }

        public Exception PrepareForTestPosition(TestPosition position)
        {
            return null;
        }

        public T GetSetupResultAt<T>(TestPosition position)
        {
            return default(T);
        }

        public IEnumerable<string> GetTraceMessages()
        {
            return new List<string>();
        }
    }
}
