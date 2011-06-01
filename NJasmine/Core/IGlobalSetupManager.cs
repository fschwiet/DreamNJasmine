namespace NJasmine.Core
{
    public interface IGlobalSetupManager
    {
        void Cleanup();
        void PrepareForTestPosition(TestPosition position);
        T GetSetupResultAt<T>(TestPosition position);
    }
}