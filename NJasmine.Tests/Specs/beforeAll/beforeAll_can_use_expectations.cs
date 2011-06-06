using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs.beforeAll
{
    [Explicit, RunExternal(false, VerificationScript = @"

param ($consoleOutput, $xmlFile);

import-module .\lib\PSUpdateXML\PSUpdateXML.psm1

update-xml $xmlFile {

    $null = get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.Specs.beforeAll.beforeAll_can_use_expectations, works'][@result='Success']"" 

    $null = get-xml -exactlyOnce ""//test-case[@name='NJasmineTests.Specs.beforeAll.beforeAll_can_use_expectations, when using expect within beforeAll, fails'][@result='Error']"" 

}
")]
    public class beforeAll_can_use_expectations : GivenWhenThenFixture
    {
        public override void Specify()
        {
            beforeAll(delegate
            {
                expect(() => true);
            });

            it("works", delegate
            {

            });

            when("using expect within beforeAll", delegate
            {
                beforeAll(delegate
                {
                    expect(() => false);
                });

                it("fails", delegate
                {

                });
            });
        }
    }
}
