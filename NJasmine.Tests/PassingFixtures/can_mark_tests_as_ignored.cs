using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace NJasmineTests.PassingFixtures
{
    [RunExternal(true, VerificationScript = @"

param ($consoleOutput, $xmlFile);

import-module .\lib\PSUpdateXML\PSUpdateXML.psm1

update-xml $xmlFile {
    
    Assert (get-xml -exactlyOnce ""//test-suite[@name='can_mark_tests_as_ignored, ignore is set before all the tests']/@result"").Equals('Inconclusive') 'Could not find first ignored suite'
    Assert (get-xml -exactlyOnce ""//test-suite[@name='can_mark_tests_as_ignored, ignore is set after a test, the earlier test is not ignored']/@result"").Equals('Success') 'Test did not succeed as expected'
    Assert (get-xml -exactlyOnce ""//test-suite[@name='can_mark_tests_as_ignored, ignore is set after a test, the later test is ignored']/@result"").Equals('Inconclusive') 'Last test did not fail as expected'
}
")]
    public class can_mark_tests_as_ignored : GivenWhenThenFixture
    {
        public override void Specify()
        {
            when("ignore is set before all the tests", delegate
            {
                ignoreBecause("the test requires it");

                then("the given block should be considered explicit", delegate
                {
                });
            });


            when("ignore is set after a test", delegate
            {
                then("the earlier test is not ignored", delegate
                {
                });

                ignoreBecause("the test requires it");

                then("the later test is ignored", delegate
                {
                });
            });
        }
    }
}
