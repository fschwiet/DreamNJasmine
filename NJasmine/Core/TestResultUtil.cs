using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NJasmine.Core
{
    public class TestResultUtil
    {
        public static void Error(TestResultShim testResult, string multilineName, Exception exception, IEnumerable<string> traceMessages, TestResultShim.Site failureSite = TestResultShim.Site.Test)
        {
            traceMessages = traceMessages ?? new List<string>();

            testResult.SetError(BuildMessage(exception), BuildStackTrace(exception, multilineName, traceMessages), failureSite);
        }

        private static string BuildStackTrace(Exception exception, string multilineName, IEnumerable<string> traceMessages)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SPECIFICATION:");
            foreach (var line in multilineName.Split('\n'))
                sb.AppendLine("    " + line);

            sb.AppendLine();

            if (traceMessages.Count() > 0)
            {
                foreach (var line in traceMessages)
                    sb.AppendLine(line);

                sb.AppendLine();
            }

            sb.AppendLine(GetStackTrace(exception));

            for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                sb.Append(Environment.NewLine);
                sb.Append("--");
                sb.Append(innerException.GetType().Name);
                sb.Append(Environment.NewLine);
                sb.Append(GetStackTrace(innerException));
            }
            return sb.ToString();
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
            StringBuilder sb = new StringBuilder();

            try
            {
                string[] stackTrace = exception.StackTrace.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).ToArray();

                bool filterNJasmineInternals = !stackTrace.Any(l => l.StartsWith("at NJasmineTests"));  // don't filter for NJasmine's tests

                foreach(var line in stackTrace)
                {
                    if (filterNJasmineInternals)
                    {
                        var lineIsInNJasmineCore = line.Trim().StartsWith("at NJasmine.Core");

                        if (lineIsInNJasmineCore)
                            continue;

                        if (PatternForNJasmineAnonymousMethod.IsMatch(line))
                            continue;
                    }

                    var match = Regex.Match(line, @"^(\s*at )(.*\\)([^\\]*\.cs:line \d+)$");

                    if (match.Success)
                    {
                        sb.AppendLine(match.Groups[1].Value + match.Groups[3].Value.Replace(":line ", ":") + "  " + match.Groups[2].Value + match.Groups[3].Value);
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
            }
            catch (Exception)
            {
                sb.Append("No stack trace available");
            }

            return sb.ToString();
        }

        public static Regex PatternForNJasmineAnonymousMethod = new Regex(@"NJasmine\..*\.<>");
    }
}
