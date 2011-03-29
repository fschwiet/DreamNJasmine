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
                "System.InvalidOperationException : Called it() within arrange().",
                "System.InvalidOperationException : Called it() within arrange()."
        })]
    public class cannot_reenter_during_arrange : GivenWhenThenFixture
    {
        public override void Specify()
        {
            describe("when the arrange code tries to re-enter", delegate
            {
                arrange(delegate()
                {
                    it("has test within arrange", delegate { });
                });

                it("has a valid test that will now fail", delegate()
                {
                });
            });

            describe("when the arrange cleanup code tries to re-enter", delegate
            {
                var fail = arrange(() => 
                    new ActOnDispose(() => this.it("test within arrange()d dispose", delegate { })));

                it("has a valid test that will now fail", delegate()
                {
                });
            });
        }

        public class ActOnDispose : IDisposable
        {
            readonly Action _action;

            public ActOnDispose(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }
    }
}
