using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    public class can_detect_ExplicitAttribute_reason : GivenWhenThenFixture
    {
        class ClassWithoutExplicitAttribute
        {
        }

        [Explicit]
        class ClassWithExplicitAttribute
        {
        }

        [Explicit("actually was dolan")]
        class ClassWithExplicitAttributeReason
        {
        }

        public override void Specify()
        {
            it("can detect a class does not have ExplicitAttribute", () =>
            {
                expect(() => ExplicitAttributeReader.GetFor(typeof (ClassWithoutExplicitAttribute)) == null);
            });

            it("can detect a class does have Explicit Attribute", () =>
            {
                expect(() => ExplicitAttributeReader.GetFor(typeof(ClassWithExplicitAttribute)) == "ClassWithExplicitAttribute has attribute ExplicitAttribute.");
            });

            it("can detect the reason attribute of ExplicitAttribute when available", () =>
            {
                expect(() => ExplicitAttributeReader.GetFor(typeof(ClassWithExplicitAttributeReason)) == "actually was dolan");
            });
        }
    }
}
