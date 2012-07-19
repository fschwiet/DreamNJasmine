using System;
using System.Collections.Generic;
using System.Linq;


namespace NJasmine.Core
{
    public class TestBuilder
    {
        readonly INativeTest _nativeTest;

        public TestBuilder(INativeTest nativeTest, TestName name = null)
        {
            _nativeTest = nativeTest;
            Children = new List<TestBuilder>();
            Categories = new List<string>();
            Name = name ?? new TestName();
        }

        public TestName Name { get; set; }

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

        public INativeTest GetUnderlyingTest()
        {
            return _nativeTest;
        }
    }
}
