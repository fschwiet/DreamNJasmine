using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmineTests
{
    interface INJasmineInternalRequirement
    {
        void Verify(bool testPassed, string outputFile, string xmlOutputFile);
    }
}
