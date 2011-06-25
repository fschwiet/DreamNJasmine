using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit, RunExternal(false, VerificationScript = @"

param ($consoleOutput, $xmlFile);

import-module .\tools\PSUpdateXML.psm1

update-xml $xmlFile {

    $null = get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.Specs.expect, can pass tests'][@result='Success']""
    $null = get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.Specs.expect, can fail tests'][@result='Error']""
    $null = get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.Specs.expect, expect can be called during discovery, doesnt prevent discovery'][@result='Error']""

}
")]
    public class expect : GivenWhenThenFixture
    {
        public override void Specify()
        {
            it("can pass tests", delegate
            {
                expect(() => true);
            });

            it("can fail tests", delegate
            {
                expect(() => false);
            });

            describe("expect can be called during discovery", delegate
            {
                expect(() => true);
                expect(() => false);

                it("doesnt prevent discovery", delegate
                {
                });
            });
        }
    }
}
