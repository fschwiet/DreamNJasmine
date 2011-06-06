using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs.beforeAll
{
    public class beforeAll_can_use_expectations : GivenWhenThenFixture
    {
        public override void Specify()
        {
            beforeAll(delegate
            {
                expect(() => true);
            });

            when("using expect within beforeAll", delegate
            {
                beforeAll(delegate
                {
                    expect(() => true);
                });

                it("works", delegate
                {

                });
            });
        }
    }
}
