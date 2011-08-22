using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    public class can_trace_help_message_for_failure_case : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            describe("trace information is not included on success", delegate
            {
                trace("a b c");

                it("is a passing test", delegate
                {
                });
            });

            describe("trace information is included on failure", delegate
            {
                trace("1 2 3");

                beforeAll(delegate
                {
                    trace("d e f");
                });

                beforeEach(delegate
                {
                    trace("g h i");
                });

                it("is a failing test", delegate
                {
                    trace("j k l");

                    expect(() => true == false);
                    //throw new Exception("hi");
                });
            });
        }

        public void Verify(FixtureResult fixtureResult)
        {
            fixtureResult.hasTest("NJasmineTests.Specs.can_trace_help_message_for_failure_case, trace information is not included on success, is a passing test")
                .thatSucceeds();

            fixtureResult.hasTest("NJasmineTests.Specs.can_trace_help_message_for_failure_case, trace information is included on failure, is a failing test")
                .thatErrors()
                .withExceptionDetail("1 2 3")
                //.withExceptionDetail("d e f")
                .withExceptionDetail("g h i")
                .withExceptionDetail("j k l");
        }
    }
}
