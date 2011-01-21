using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;
using Should.Fluent;

namespace NJasmineTests.PassingFixtures
{
    public class constructor_is_run_once_per_test : NJasmineFixture
    {
        public int? LastValue;
        public constructor_is_run_once_per_test LastInstance;
        public int Value = 0;

        public override void Specify()
        {
            for (var i = 0; i < 10; i++)
            {
                it("no state is saved from a previous run", () => LastValue.HasValue.Should().Be.False());

                it("new instance is created per test", () => LastInstance.Should().Be.Null());

                it("writes state in some tests", delegate
                {
                    LastValue = 1;
                    LastInstance = this;
                });
            }
        }
    }
}
