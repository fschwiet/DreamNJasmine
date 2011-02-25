using System;

namespace NJasmine.Core
{
    public class RunsActionOnDispose : IDisposable
    {
        Action _action;

        public RunsActionOnDispose(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            if (_action != null)
            {
                _action();
                _action = null;
            }
        }
    }
}