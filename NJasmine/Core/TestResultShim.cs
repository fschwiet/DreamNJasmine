using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public class TestResultShim
    {
        private readonly TestResult _testResult;

        public TestResultShim(TestResult testResult = null)
        {
            _testResult = testResult ?? new TestResult(new TestName());
        }

        public void Success()
        {
            _testResult.Success();
        }

        public bool IsSuccess { get { return _testResult.IsSuccess;  } }
    }
}
