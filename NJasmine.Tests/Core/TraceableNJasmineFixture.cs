using System;
using System.Linq;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    public abstract class TraceableNJasmineFixture : NJasmineFixture
    {
        public class PerClassTraceResetFixture
        {
            [TestFixtureSetUp]
            public void TestFixtureSetup()
            {
                TraceReset();
            }
        }

        public static void Trace(string value)
        {
            Console.WriteLine("<<{{" + value + "}}>>");
        }

        public static void TraceReset()
        {
            Console.WriteLine("{{<<RESET>>}}");
        }

        public static string GetTypeShortName(Type type)
        {
            return type.ToString().Split(new char[] {'+', '.'}).Last();
        }

        protected void ResetTracing()
        {
            importNUnit<PerClassTraceResetFixture>();
        }
    }
}