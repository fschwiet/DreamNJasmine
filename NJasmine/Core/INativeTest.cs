namespace NJasmine.Core
{
    public interface INativeTest
    {
        void AddChild(TestBuilder test);
        void SetIgnoreReason(string reasonIgnored);
    }
}