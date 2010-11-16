using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should.Fluent;

namespace NJasmineTests.Integration
{
    public class RunExternalAttribute : Attribute
    {
        public bool TestPasses { get; set; }
        public string[] ExpectedStrings { get; set; }
        public string ExpectedExtraction { get; set; }

        public RunExternalAttribute(bool testPasses)
        {
            TestPasses = testPasses;
        }

        public static IEnumerable<object> GetAll()
        {
            return (from t in typeof (RunExternalAttribute).Assembly.GetTypes()
                    where t.IsDefined(typeof (RunExternalAttribute), false)
                    from a in t.GetCustomAttributes(typeof (RunExternalAttribute), false).Cast<RunExternalAttribute>()
                    select new
                    {
                        Name = t.FullName,
                        Passes = a.TestPasses,
                        ExpectedStrings = a.ExpectedStrings ?? new string[0],
                        ExpectedExtraction = a.ExpectedExtraction
                    }
                    ).Cast<Object>();
        }
    }
}