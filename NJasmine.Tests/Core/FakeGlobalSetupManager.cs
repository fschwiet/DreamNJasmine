using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core;

namespace NJasmineTests.Core
{
    class FakeGlobalSetupManager : IGlobalSetupManager
    {
        public void Cleanup()
        {
        }

        public void PrepareForTestPosition(TestPosition position)
        {
        }

        public T GetSetupResultAt<T>(TestPosition position)
        {
            return default(T);
        }
    }
}
