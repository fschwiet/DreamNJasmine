using System;
using System.Collections.Generic;

namespace NJasmine.Core
{
    public interface IGlobalSetupManager
    {
        void Cleanup(TestPosition position);
        void PrepareForTestPosition(TestPosition position, out Exception existingError);
        T GetSetupResultAt<T>(TestPosition position);
        IEnumerable<string> GetTraceMessages();
    }
}