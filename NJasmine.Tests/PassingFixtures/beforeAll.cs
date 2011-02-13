using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;
using NJasmineTests.Core;
using NUnit.Framework;

namespace NJasmineTests.PassingFixtures
{
    [Explicit]
    [RunExternal(true, ExpectedTraceSequence = @"
BEFORE ALL
SECOND BEFORE ALL
INNER BEFORE ALL
FINAL BEFORE ALL
first test 
second test
third test
FINAL AFTER ALL
INNER AFTER ALL
SECOND AFTER ALL
AFTER ALL
")]
    public class beforeAll : TraceableNJasmineFixture
    {
        public override void Specify()
        {
            beforeAll(ResetTracing);

            SpecVisitor.visitBeforeAll(SpecElement.beforeAll, delegate
            {
                Trace("BEFORE ALL");
                return (string)null;
            });

            SpecVisitor.visitAfterAll(SpecElement.afterAll, delegate
            {
                Trace("AFTER ALL");
            }); 
            
            SpecVisitor.visitTest(SpecElement.it, "first teest", delegate
            {
                Trace("first test");
            });

            SpecVisitor.visitBeforeAll(SpecElement.beforeAll, delegate
            {
                Trace("SECOND BEFORE ALL");
                return (string)null;
            });

            SpecVisitor.visitAfterAll(SpecElement.afterAll, delegate
            {
                Trace("SECOND AFTER ALL");
            });

            SpecVisitor.visitFork(SpecElement.describe, "in some context", delegate
            {
                SpecVisitor.visitBeforeAll(SpecElement.beforeAll, delegate
                {
                    Trace("INNER BEFORE ALL");
                    return (string)null;
                });

                SpecVisitor.visitAfterAll(SpecElement.afterAll, delegate
                {
                    Trace("INNER AFTER ALL");
                });

                SpecVisitor.visitTest(SpecElement.it, "second teest", delegate
                {
                    Trace("second test");
                });

                SpecVisitor.visitTest(SpecElement.it, "third teest", delegate
                {
                    Trace("third test");
                });

            });

            SpecVisitor.visitBeforeAll(SpecElement.beforeAll, delegate
            {
                Trace("FINAL BEFORE ALL");
                return (string)null;
            });

            SpecVisitor.visitAfterAll(SpecElement.afterAll, delegate
            {
                Trace("FINAL AFTER ALL");
            }); 
        }
    }
}
