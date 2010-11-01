using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine;

namespace NJasmineTests.Core
{
    public abstract class ObservableNJasmineFixture : NJasmineFixture
    {
        List<string> _observations = new List<string>();

        public List<string> Observations
        {
            get { return _observations; }
        }

        public void Observe(string value)
        {
            _observations.Add(value);
        }

        public static void Trace(string value)
        {
            Console.WriteLine("<<{{" + value + "}}>>");
        }

        public static string GetTypeShortName(Type type)
        {
            return type.ToString().Split(new char[] {'+', '.'}).Last();
        }
    }
}