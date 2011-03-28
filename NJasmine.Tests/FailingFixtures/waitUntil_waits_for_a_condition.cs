using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, VerificationScript = @"

param ($consoleOutput, $xmlFile);

import-module .\lib\PSUpdateXML\PSUpdateXML.psm1

update-xml $xmlFile {

    get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.FailingFixtures.waitUntil_waits_for_a_condition, given a condition that eventually evalutes to true, a normal expect works when no waits are left'][@result='Success']""
    get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.FailingFixtures.waitUntil_waits_for_a_condition, given a condition that eventually evalutes to true, a normal expect fails when waits are left'][@result='Error']""
    get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.FailingFixtures.waitUntil_waits_for_a_condition, given a condition that eventually evalutes to true, waitUntil will try multiple times'][@result='Success']""
}
")]
    public class waitUntil_waits_for_a_condition : NJasmineFixture
    {
        public int WaitsLeft;

        public bool Ready()
        {
            return WaitsLeft-- == 0;
        }

        public override void Specify()
        {
            describe("given a condition that eventually evalutes to true", delegate
            {
                it("a normal expect works when no waits are left", delegate
                {
                    WaitsLeft = 0;
                    expect(() => Ready());
                });

                it("a normal expect fails when waits are left", delegate
                {
                    WaitsLeft = 1;
                    expect(() => Ready());
                });

                it("waitUntil will try multiple times", delegate
                {
                    WaitsLeft = 4;
                    waitUntil(() => Ready());
                });
            });

            it("fails", delegate
            {
                expect(() => false);
            });
        }
    }
}
