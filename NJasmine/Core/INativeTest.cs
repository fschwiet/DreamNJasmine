namespace NJasmine.Core
{
    public interface INativeTest
    {
        void AddChild(TestBuilder test);
        void MarkTestIgnored(string reasonIgnored);
        void MarkTestInvalid(string reason);
    }
}