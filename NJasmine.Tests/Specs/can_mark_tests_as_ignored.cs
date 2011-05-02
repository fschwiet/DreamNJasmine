using NJasmine;

namespace NJasmineTests.Specs
{
    [RunExternal(true, VerificationScript = @"

param ($consoleOutput, $xmlFile);

import-module .\lib\PSUpdateXML\PSUpdateXML.psm1

update-xml $xmlFile {

    for-xml -exactlyOnce ""//test-suite[@name='given an outer block']"" {
        Assert ((get-xml ""@result"") -eq 'Inconclusive') 'Expected first outer block to be inconclusive';

        for-xml ""./results"" {
            Assert $false 'Expected first outer block to not have any subresults'
        }
    }
    
    for-xml -exactlyOnce ""//test-suite[@name='when ignore is set after a test']"" {
        Assert ((get-xml ""@result"") -eq 'Success') 'Expected when statement with non-ignored test to have succeeded.';

        for-xml -exactlyOnce ""./results/test-case"" {
            Assert ((get-xml ""@name"") -eq 'NJasmineTests.Specs.can_mark_tests_as_ignored, when ignore is set after a test, then the earlier test runs') `
                'Expected when statement with non-ignored test to contain the non-ignored test'

            Assert ((get-xml ""@result"") -eq 'Success') 'Expected non-ignored test to have passed';
        }
    }
}
")]
    public class can_mark_tests_as_ignored : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("an outer block", delegate
            {
                when("ignore is set before all the tests", delegate
                {
                    ignoreBecause("the test requires it");

                    then("the when statement doesnt run, and the outer block is inconclusive", delegate
                    {
                    });
                });
            });

            when("ignore is set after a test", delegate
            {
                then("the earlier test runs", delegate
                {
                });

                ignoreBecause("the test requires it");

                then("the later test is ignored, and the containing when is inconclusive", delegate
                {
                });
            });
        }
    }
}
