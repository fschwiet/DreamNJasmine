using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Core;

namespace NJasmineTests.PassingFixtures
{
    [RunExternal(true, ExpectedTraceSequence = @"
one
two
three
four
-four
-three
-two
-one
")]
    public class arrange_operations_can_be_nested : TraceableNJasmineFixture
    {
        public override void Tests()
        {
            importNUnit<TraceableNJasmineFixture.PerClassTraceResetFixture>();

            beforeEach(delegate
            {
                arrange(() => Trace("one"));
                afterEach(() => Trace("-one"));
            });

            arrange(delegate
            {
                beforeEach(delegate
                {
                    arrange(delegate
                    {
                        beforeEach(() => Trace("two"));
                        afterEach(() => Trace("-two"));
                    });
                });
            });

            it("has a test", delegate
            {
                arrange(delegate
                {
                    beforeEach(delegate
                    {
                        beforeEach(delegate
                        {
                            arrange(() => Trace("three"));
                            afterEach(() => Trace("-three"));
                        });
                    });
                });

                arrange(delegate
                {
                    beforeEach(() => Trace("four"));
                    afterEach(() => Trace("-four"));
                });
            });
        }
    }
}
