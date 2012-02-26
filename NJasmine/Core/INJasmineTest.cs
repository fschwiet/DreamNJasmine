using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public interface INJasmineNameable
    {
        string Shortname { get; set; }
        string FullName { get; set; }
        string MultilineName { get; set; }
    }

    public interface INJasmineTest : ITest, INJasmineNameable
    {
        TestPosition Position { get; }
    }
}
