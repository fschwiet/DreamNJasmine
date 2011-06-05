using System;

namespace NJasmine.Core
{
    public interface IGlobalSetupManager
    {
        void Cleanup(TestPosition position);
        void PrepareForTestPosition(TestPosition position, out Exception existingError);
        T GetSetupResultAt<T>(TestPosition position);
    }
}