using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineBuildResult
    {
        private readonly Test _test;

        public NJasmineBuildResult(Test test)
        {
            _test = test;
        }

        public Test GetNUnitResult()
        {
            return _test;
        }

        public bool IsSuite()
        {
            return _test.IsSuite;
        }
    }
}
