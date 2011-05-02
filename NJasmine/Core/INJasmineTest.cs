using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public interface INJasmineTest : ITest
    {
        TestPosition Position { get; }
    }
}
