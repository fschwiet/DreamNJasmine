using System;

namespace NJasmine.Core.NativeWrappers
{
    public interface INativeTest
    {
        TestName Name { get; }
        void AddCategory(string category);
        void AddChild(TestBuilder test);
        void MarkTestIgnored(string reasonIgnored);
        void MarkTestInvalid(string reason);
        void MarkTestFailed(Exception exception);
        object GetNative();
    }
}