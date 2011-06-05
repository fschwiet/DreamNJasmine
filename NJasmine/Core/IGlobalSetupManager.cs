using System;

namespace NJasmine.Core
{
    public interface IGlobalSetupManager
    {
        void Cleanup();
        void PrepareForTestPosition(TestPosition position, out Exception existingError);
        T GetSetupResultAt<T>(TestPosition position);
    }
}