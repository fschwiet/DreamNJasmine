using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Marshalled;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests
{
    public class XzibitTest
    {
        [Test]
        public void AllSpecificationsPass()
        {
            var testNames = Executor.SpecEnumerator.GetTestNames(GetType().Assembly, t => typeof(INJasmineInternalRequirement).IsAssignableFrom(t));

            foreach(var testName in testNames)
            {
                Console.WriteLine(testName);
            }
        }
    }
}
