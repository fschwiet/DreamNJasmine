using System;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs.beforeAll
{
    [Explicit]
    [RunExternal(false, VerificationScript = @"

param ($consoleOutput, $xmlFile);

import-module .\tools\PSUpdateXML.psm1

update-xml $xmlFile {

    function assertTestHasMessage($testName, $expectedMessage) {

        for-xml -exactlyOnce ""//test-case[@name='$testName']"" {
    
            $errorMessage = get-xml -exactlyOnce './/message'

            Assert ($errorMessage -like ""*$expectedMessage*"") ""Expected $testName to have error message $expectedMessage.""
        }
    }

    assertTestHasMessage 'NJasmineTests.Specs.beforeAll.beforeAll_doesnt_reexecute, then reports the test with the correct count'   'Failed with TotalRuns: 1'
    assertTestHasMessage 'NJasmineTests.Specs.beforeAll.beforeAll_doesnt_reexecute, then reports the test with the correct count`2' 'Failed with TotalRuns: 1'
    assertTestHasMessage 'NJasmineTests.Specs.beforeAll.beforeAll_doesnt_reexecute, then reports the test with the correct count`3' 'Failed with TotalRuns: 1'
    assertTestHasMessage 'NJasmineTests.Specs.beforeAll.beforeAll_doesnt_reexecute, then reports the test with the correct count`4' 'Failed with TotalRuns: 1'
}

")]
    public class beforeAll_doesnt_reexecute : GivenWhenThenFixture
    {
        public static int TotalRuns = 0;

        public override void Specify()
        {
            beforeAll(() =>
            {
                TotalRuns++;

                throw new Exception("Failed with TotalRuns: " + TotalRuns);
            });

            it("then reports the test with the correct count", delegate { });
            it("then reports the test with the correct count", delegate { });
            it("then reports the test with the correct count", delegate { });
            it("then reports the test with the correct count", delegate { });
        }
    }
}
