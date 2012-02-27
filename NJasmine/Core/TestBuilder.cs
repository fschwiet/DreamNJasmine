using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public class TestBuilder
    {
        readonly NativeTest _nativeTest;

        public TestBuilder(NativeTest nativeTest)
        {
            _nativeTest = nativeTest;
            Children = new List<TestBuilder>();
            Categories = new List<string>();
        }

        public string Shortname { get; set; }
        public string FullName { get; set; }
        public string MultilineName { get; set; }

        public string ReasonIgnored { get; private set; }

        public List<TestBuilder> Children { get; private set; }
        public List<string> Categories { get; private set; }

        public void AddChildTest(TestBuilder test)
        {
            Children.Add(test);
        }

        public void AddCategory(string category)
        {
            Categories.Add(category);
        }

        public void AddIgnoreReason(string ignoreReason)
        {
            if (String.IsNullOrEmpty(ReasonIgnored))
                ReasonIgnored = ignoreReason;
            else
                ReasonIgnored = ReasonIgnored + ", " + ignoreReason;
        }

        public Test GetUnderlyingTest()
        {
            return _nativeTest.GetNative(this);
        }
    }
}
