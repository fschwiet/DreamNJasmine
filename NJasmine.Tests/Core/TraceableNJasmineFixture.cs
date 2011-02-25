using System;
using System.Linq;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    public abstract class TraceableNJasmineFixture : NJasmineFixture
    {
        public void ExtendSpec(Action<ISpecVisitor> specVisitor)
        {
            GetUnderlyingSkelefixture(this).ExtendSpec(specVisitor);
        }

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