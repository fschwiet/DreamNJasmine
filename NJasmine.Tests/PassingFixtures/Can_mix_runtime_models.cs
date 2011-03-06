using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Core;

namespace NJasmineTests.PassingFixtures
{
    public class TestDriver
    {
        NJasmineContext _njasmineContext;
        GivenWhenThenContext _gwtTestContext;

        public TestDriver(SpecificationFixture fixture)
        {
            _njasmineContext = new NJasmineContext(SpecificationFixture.GetUnderlyingSkelefixture(fixture));
            _gwtTestContext = new GivenWhenThenContext(SpecificationFixture.GetUnderlyingSkelefixture(fixture));
        }

        public int Value = 0;

        public void do_arrange_in_njasmine()
        {
            _njasmineContext.beforeAll(() => TraceableNJasmineFixture.ResetTracing());
            
            _njasmineContext.arrange(delegate
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
            var testDriver = new TestDriver(this);

            testDriver.do_arrange_in_njasmine();

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
            var testDriver = new TestDriver(this);

            testDriver.do_arrange_in_njasmine();

            testDriver.do_test_with_gwt("the test");
        }
    }
}
