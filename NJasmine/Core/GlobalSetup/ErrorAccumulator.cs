using System;

namespace NJasmine.Core.GlobalSetup
{
    public class ErrorAccumulator
    {
        protected TestPosition _existingErrorPosition;
        protected Exception _existingError;

        public Exception GetErrorForPosition(TestPosition position)
        {
            if (_existingError != null && _existingErrorPosition != null && _existingErrorPosition.IsOnPathTo(position))
                return _existingError;
            else
                return null;
        }

        public void AddError(TestPosition position, Exception error)
        {
            _existingError = error;
            _existingErrorPosition = position;
        }
    }
}