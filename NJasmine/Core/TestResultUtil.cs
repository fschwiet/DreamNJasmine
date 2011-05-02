using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public class TestResultUtil
    {
        public static void Error(TestResult testResult, Exception exception)
        {
            SetResult(testResult, ResultState.Error, exception, FailureSite.Test);
        }

        private static void SetResult(TestResult testResult, ResultState resultState, Exception ex, FailureSite failureSite)
        {
            if (resultState == ResultState.Cancelled)
            {
                testResult.SetResult(resultState, "Test cancelled by user", BuildStackTrace(ex), failureSite);
            }
            else
            {
                if (resultState == ResultState.Error)
                {
                    testResult.SetResult(resultState, BuildMessage(ex), BuildStackTrace(ex), failureSite);
                }
                else
                {
                    testResult.SetResult(resultState, ex.Message, ex.StackTrace, failureSite);
                }
            }
        }

        private static string BuildStackTrace(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder(GetStackTrace(exception));
            for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("--");
                stringBuilder.Append(innerException.GetType().Name);
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(GetStackTrace(innerException));
            }
            return stringBuilder.ToString();
        }

        private static string BuildMessage(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} : {1}", exception.GetType().ToString(), exception.Message);
            for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.AppendFormat("  ----> {0} : {1}", innerException.GetType().ToString(), innerException.Message);
            }
            return stringBuilder.ToString();
        }

        private static string GetStackTrace(Exception exception)
        {
            string result;
            try
            {
                result = exception.StackTrace;
            }
            catch (Exception)
            {
                result = "No stack trace available";
            }
            return result;
        }
    }
}
