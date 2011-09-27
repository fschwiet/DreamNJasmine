using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NJasmineTests.Specs.proposed_specs.inline_branching;
using NUnit.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    public class NJasmineSuiteBuilder_can_build_suite_with_fork
    {
        [Test]
        public void run()
        {
            var builder = new NJasmineSuiteBuilder();

            var suite = builder.BuildFrom(typeof(can_branch_inline));

            var names = GetTestNames(suite).ToArray();
            Assert.That(names[0], Is.StringEnding("given some precondition, when the input is 0, then it runs"));
            Assert.That(names[1], Is.StringEnding("given some precondition, when the input is 1, then it runs"));
        }

        IEnumerable<string> GetTestNames(ITest test)
        {
            List<string> names = new List<string>();

            if (test is TestMethod)
            {
                names.Add((test as TestMethod).TestName.FullName);
            }

            if (test.Tests != null)
            {
                foreach (var subtest in test.Tests.OfType<ITest>())
                {
                    names.AddRange(GetTestNames(subtest));
                }
            }

            return names;
        }
    }
}
