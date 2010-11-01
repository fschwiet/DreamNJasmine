using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NUnit.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    [TestFixture]
    public class NJasmineInvalidTestSuiteTests : ExpectationsFixture
    {
        [Test]
        public void Report_test_failure_with_reason()
        {
            string locationOfFailure = "123abc";
            Exception exception = null;

            try
            {
                int i = 0;
                int j = 1/i;
            }
            catch (Exception e)
            {
                exception = e;
            }

            var sut = new NJasmineInvalidTestSuite("someString", locationOfFailure, exception, null);

            TestResult result = new TestResult(sut);

            sut.RunTestMethod(result);

            expect(result.IsSuccess).to.Equal(false);
            expect(result.Message).to.Equal(locationOfFailure + ": " + exception.Message);
            expect(result.StackTrace).to.Equal(exception.StackTrace);
        }
    }
}
