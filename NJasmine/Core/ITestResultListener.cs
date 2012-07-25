namespace NJasmine.Core
{
    public interface ITestResultListener
    {
        void NotifyStart(string name);
        void NotifyEnd(string name);
    }
}