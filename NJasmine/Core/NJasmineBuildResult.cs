using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineBuildResult : INJasmineNameable
    {
        private Test _test;

        public NJasmineBuildResult(Test test)
        {
            _test = test;
        }

        public void ReplaceNUnitResult(Test test)
        {
            test.TestName.Name = Shortname;
            test.TestName.FullName = FullName;
            test.SetMultilineName(MultilineName);

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

        public string Shortname
        {
            get { return _test.TestName.Name; }
            set { _test.TestName.Name = value; }
        }

        public string FullName
        {
            get { return _test.TestName.FullName; }
            set { _test.TestName.FullName = value; }
        }

        public string MultilineName
        {
            get { return TestExtensions.GetMultilineName(_test); }
            set { TestExtensions.SetMultilineName(_test, value); }
        }

        public void SetIgnoreReason(string reason)
        {
            if (string.IsNullOrEmpty(this._test.IgnoreReason))
            {
                _test.RunState = RunState.Explicit;
                _test.IgnoreReason = reason;
            }
        }

        public void AddChildTest(NJasmineBuildResult test)
        {
            (_test as TestSuite).Add(test.GetNUnitResult());
        }
    }
}
