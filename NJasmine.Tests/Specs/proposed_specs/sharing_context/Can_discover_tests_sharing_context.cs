using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NUnit.Framework;

namespace NJasmineTests.Specs.proposed_specs.sharing_context
{
    public class Can_discover_tests_sharing_context : GivenWhenThenFixture
    {
        public override void Specify()
        {
            var testFactory = new GenericTestFactory();
            var type = typeof (SomeClassUsingContext);

            it("can discover tests sharing context", () =>
            {
                using (SpecificationBuilder.BuildTestFixture(type, testFactory))
                {
                    Assert.That(testFactory.Names, Is.EquivalentTo(new string[]
                {
                    "NJasmineTests.Specs.proposed_specs.sharing_context.SomeClassUsingContext.given a web browser, it can use the context"
                }));
                }
            });
        }
    }
}
