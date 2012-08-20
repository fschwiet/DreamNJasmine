using System;
using System.Collections.Generic;

namespace NJasmine.Core.GlobalSetup
{
    public interface IGlobalSetupManager
    {
        void Cleanup(TestPosition position);
        Exception PrepareForTestPosition(TestPosition position);
        T GetSetupResultAt<T>(TestPosition position);
        IEnumerable<string> GetTraceMessages();
    }
}