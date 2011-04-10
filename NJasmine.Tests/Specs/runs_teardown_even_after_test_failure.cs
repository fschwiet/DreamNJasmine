﻿using System;
using NJasmineTests.Core;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit, RunExternal(false, ExpectedTraceSequence = @"
failure_in_test_doesnt_prevent_cleanup_in_same_scope
failure_in_test_doesnt_prevent_cleanup")]

    public class runs_teardown_even_after_test_failure : GivenWhenThenFixtureTracingToConsole
    {
        public override void Specify()
        {
            beforeAll(ResetTracing);

            afterEach(delegate()
            {
                Trace("failure_in_test_doesnt_prevent_cleanup");
            });

            describe("failng describe", delegate()
            {
                afterEach(delegate()
                {
                    Trace("failure_in_test_doesnt_prevent_cleanup_in_same_scope");
                });

                it("failing test", delegate()
                {
                    throw new Exception("intended test failure");
                });
            });
        }
    }
}