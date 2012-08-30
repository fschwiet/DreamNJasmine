using System;
using NJasmine.Core.NativeWrappers;

namespace NJasmine.Core
{
    public class GenericNativeTest : INativeTest
    {
        public TestName Name { get; private set; }
        public string ReasonIgnored;
    
        public GenericNativeTest(TestName name)
        {
            Name = name;
        }

        public void AddCategory(string category)
        {
        }

        public void AddChild(INativeTest test)
        {
        }

        public void MarkTestIgnored(string reasonIgnored)
        {
            ReasonIgnored = reasonIgnored;
        }

        public void MarkTestInvalid(string reason)
        {
        }

        public void MarkTestFailed(Exception exception)
        {
        }

        public object GetNative()
        {
            return null;
        }
    }
}