using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineNUnitTestMethod : TestMethod, INJasmineTest
    {
        public NJasmineNUnitTestMethod(MethodInfo method, TestPosition position) : base(method)
        {
            Position = position;
        }

        public string Shortname
        {
            get { return this.TestName.Name; }
            set { this.TestName.Name = value; }
        }

        public string FullName
        {
            get { return this.TestName.FullName; }
            set { this.TestName.FullName = value; }
        }

        public string MultilineName
        {
            get { return TestExtensions.GetMultilineName(this); }
            set { TestExtensions.SetMultilineName(this, value); }
        }

        public TestPosition Position { get; private set; }
    }
}
