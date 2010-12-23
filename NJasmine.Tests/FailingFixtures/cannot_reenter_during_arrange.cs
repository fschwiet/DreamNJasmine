using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit]
    [RunExternal(false,
        ExpectedStrings = new string[] {
                "Test Error : NJasmineTests.FailingFixtures.cannot_reenter_during_arrange, when the arrange code tries to re-enter, has a valid test that will now fail",
                "Test Error : NJasmineTests.FailingFixtures.cannot_reenter_during_arrange, when the arrange cleanup code tries to re-enter, has a valid test that will now fail",
                "System.InvalidOperationException : Called afterEach() within arrange().",
                "System.InvalidOperationException : Called beforeEach() within arrange()."
        })]
    public class cannot_reenter_during_arrange : NJasmineFixture
    {
        public override void Tests()
        {
            describe("when the arrange code tries to re-enter", delegate
            {
                arrange(delegate()
                {
                    afterEach(delegate { });
                });

                it("has a valid test that will now fail", delegate()
                {
                });
            });

            describe("when the arrange cleanup code tries to re-enter", delegate
            {
                var fail = arrange(() => new ReentersOnDispose(this));

                it("has a valid test that will now fail", delegate()
                {
                });
            });
        }

        public class ReentersOnDispose : IDisposable
        {
            readonly IArrangeContext _context;

            public ReentersOnDispose(IArrangeContext context)
            {
                _context = context;
            }

            public void Dispose()
            {
                _context.beforeEach(delegate { });
            }
        }
    }
}
