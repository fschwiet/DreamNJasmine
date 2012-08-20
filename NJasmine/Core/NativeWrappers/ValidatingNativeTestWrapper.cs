using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;

namespace NJasmine.Core.NativeWrappers
{
    public class ValidatingNativeTestWrapper : INativeTest
    {
        private string _reasonIgnored;
        private readonly INativeTest _nativeTest;

        public ValidatingNativeTestWrapper(INativeTest nativeTest)
        {
            _nativeTest = nativeTest;
        }

        public TestName Name { get
            {
                return _nativeTest.Name;
            } 
        }

        public void AddCategory(string category)
        {
            var invalidReason = Validate.CheckForCategoryError(category);
            if (invalidReason != null)
            {
                _nativeTest.MarkTestInvalid(invalidReason);
            }
            else
            {
                _nativeTest.AddCategory(category);
            }
        }

        public void AddChild(INativeTest test)
        {
            _nativeTest.AddChild(test);
        }

        public void MarkTestIgnored(string reasonIgnored)
        {
            if (String.IsNullOrEmpty(_reasonIgnored))
                _reasonIgnored = reasonIgnored;
            else
                _reasonIgnored = _reasonIgnored + ", " + reasonIgnored;

            _nativeTest.MarkTestIgnored(_reasonIgnored);
        }

        public void MarkTestInvalid(string reason)
        {
            _nativeTest.MarkTestInvalid(reason);
        }

        public void MarkTestFailed(Exception exception)
        {
            _nativeTest.MarkTestFailed(exception);
        }

        public object GetNative()
        {
            return _nativeTest.GetNative();
        }
    }
}
