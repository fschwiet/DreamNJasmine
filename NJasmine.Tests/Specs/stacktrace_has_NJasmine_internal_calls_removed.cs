using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    [RunExternal(false, VerificationScript = @"

param ($consoleOutput, $xmlFile);

import-module .\lib\PSUpdateXML\PSUpdateXML.psm1

update-xml $xmlFile {

    $stackTrace = get-xml -exactlyonce ""//stack-trace"";
    
    assert (-not ($stackTrace -like '*NJasmine.Core*')) 'Expected NJasmine.Core stack elements to be removed';

}
")]
    public class stacktrace_has_NJasmine_internal_calls_removed : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("some context", delegate
            {
                when("some action", delegate
                {
                    then("it fails", delegate()
                    {
                        expect(() => 1 + 2 == 4);
                    });
                });
            });
        }
    }
}
