using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Core;

namespace NJasmineTests.PassingFixtures
{
    public class TestDriver
    {
        NJasmineArrangeContext _njasmineContext;
        GivenWhenThenContext _gwtTestContext;

        public TestDriver(ISpecVisitor specVisitor)
        {
            _njasmineContext = new NJasmineArrangeContext(specVisitor);
            _gwtTestContext = new GivenWhenThenContext(specVisitor);
        }

        public int Value = 0;

        public void do_arrange_in_njasmine(string arrangeDescription)
        {
            _njasmineContext.importNUnit<TraceableNJasmineFixture.PerClassTraceResetFixture>();

            _njasmineContext.arrange(arrangeDescription, delegate
            {
                TraceableNJasmineFixture.Trace("doing arrange in NJasmine");
                Value = 2;
            });
        }

        public void do_test_with_gwt(string testName)
        {
            _gwtTestContext.then(testName, delegate
            {
                TraceableNJasmineFixture.Trace("doing test in GivenWhenThen");
                _gwtTestContext.expect(() => Value == 2);
            });
        }
    }

    [RunExternal(true, ExpectedTraceSequence = @"
doing arrange in NJasmine
doing test in GivenWhenThen
")]
    public class can_mix_runtime_models_in_NJasmine : NJasmineFixture
    {
        public override void Specify()
        {
            var testDriver = new TestDriver(SpecVisitor);

            testDriver.do_arrange_in_njasmine("the arrange");

            testDriver.do_test_with_gwt("the test");
        }
    }

    [RunExternal(true, ExpectedTraceSequence = @"
doing arrange in NJasmine
doing test in GivenWhenThen
")]
    public class can_mix_runtime_models_in_GivenWhenThen : GivenWhenThenFixture
    {
        public override void Specify()
        {
            var testDriver = new TestDriver(SpecVisitor);

            testDriver.do_arrange_in_njasmine("the arrange");

            testDriver.do_test_with_gwt("the test");
        }
    }
}
