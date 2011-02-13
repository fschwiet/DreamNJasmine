using System;
using System.Linq;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    public abstract class TraceableNJasmineFixture : NJasmineFixture
    {
        public static void Trace(string value)
        {
            Console.WriteLine("<<{{" + value + "}}>>");
        }

        public static void ResetTracing()
        {
            Console.WriteLine("{{<<RESET>>}}");
        }

        public static string GetTypeShortName(Type type)
        {
            return type.ToString().Split(new char[] {'+', '.'}).Last();
        }
    }
}