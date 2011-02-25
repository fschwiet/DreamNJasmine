using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;
using NJasmineTests.Core;
using NUnit.Framework;

namespace NJasmineTests.PassingFixtures
{
    [RunExternal(true, ExpectedTraceSequence = @"
BEFORE ALL
first test
SECOND BEFORE ALL
INNER BEFORE ALL
second test
third test
INNER AFTER ALL
DISPOSING INNER BEFORE ALL
SECOND AFTER ALL
DISPOSING SECOND BEFORE ALL
AFTER ALL
DISPOSING BEFORE ALL
")]
    public class beforeAll : TraceableNJasmineFixture
    {
        public class RunOnDispose : IDisposable
        {
            private Action _action; 

            public RunOnDispose(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
                _action = null;
            }
        }

        public override void Specify()
        {
            beforeAll(ResetTracing);

            ExtendSpec(s => s.visitBeforeAll(SpecElement.beforeAll, delegate
            {
                Trace("BEFORE ALL");
                return new RunOnDispose(() => Trace("DISPOSING BEFORE ALL"));
            }));

            ExtendSpec(s => s.visitAfterAll(SpecElement.afterAll, delegate
            {
                Trace("AFTER ALL");
            })); 
            
            ExtendSpec(s => s.visitTest(SpecElement.it, "first teest", delegate
            {
                Trace("first test");
            }));

            ExtendSpec(s => s.visitBeforeAll(SpecElement.beforeAll, delegate
            {
                Trace("SECOND BEFORE ALL");
                return new RunOnDispose(() => Trace("DISPOSING SECOND BEFORE ALL"));
            }));

            ExtendSpec(s =>s.visitAfterAll(SpecElement.afterAll, delegate
            {
                Trace("SECOND AFTER ALL");
            }));

            ExtendSpec(r => r.visitFork(SpecElement.describe, "in some context", delegate
            {
                ExtendSpec(s => s.visitBeforeAll(SpecElement.beforeAll, delegate
                {
                    Trace("INNER BEFORE ALL");
                    return new RunOnDispose(() => Trace("DISPOSING INNER BEFORE ALL"));
                }));

                ExtendSpec(s => s.visitAfterAll(SpecElement.afterAll, delegate
                {
                    Trace("INNER AFTER ALL");
                }));

                ExtendSpec(s => s.visitTest(SpecElement.it, "second teest", delegate
                {
                    Trace("second test");
                }));

                ExtendSpec(s => s.visitTest(SpecElement.it, "third teest", delegate
                {
                    Trace("third test");
                }));
            }));

            ExtendSpec(s => s.visitBeforeAll(SpecElement.beforeAll, delegate
            {
                Trace("FINAL BEFORE ALL");
                return new RunOnDispose(() => Trace("DISPOSING FINAL BEFORE ALL"));
            }));

            ExtendSpec(s => s.visitAfterAll(SpecElement.afterAll, delegate
            {
                Trace("FINAL AFTER ALL");
            })); 
        }
    }
}
