
properties {
    $base_dir  = resolve-path .
    $build_dir = "$base_dir\build\"
    $solution = "$base_dir\NJasmine.sln"
    $msbuild_Configuration = "Debug"
	$deploySource = "$base_dir\NJasmine\bin\$msbuild_Configuration\"
	$testDeployTarget = "$base_dir\packages\NUnit.2.5.7.10213\Tools\addins\"
	$testDll = "$base_dir\NJasmine.Tests\bin\$msbuild_Configuration\NJasmine.Tests.dll"
    $nunit_path = "$base_dir\packages\NUnit.2.5.7.10213\tools\nunit-console.exe"
    $localDeployTargets = (get-item 'C:\Program Files (x86)\NUnit 2.*\bin\net-2.0\') | % {$_.fullname}
    $filesToDeploy = @("NJasmine.dll", "NJasmine.pdb", "Should.Fluent.dll")
}

task default -depends AllTests

function GetAllNUnits {
    (get-item 'C:\Program Files (x86)\NUnit 2.*\bin\net-2.0\framework') | 
    % { @{ frameworkPath = $_; addinPath = (join-path (resolve-path (join-path $_ "..")) "addins") } };
}

task Build {
	exec { & $msbuild $sln /property:Configuration=$msbuild_Configuration }
}

task TestDeploy -depends Build {

	if (test-path $testDeployTarget) {
		rm $testDeployTarget -recurse
	}
	mkdir $testDeployTarget
	cp $deploySource\* $testDeployTarget -recurse
}

task LocalDeploy -depends AllTests {

    if (-not $localDeployTargets) {
        "Local deploy target not found." | write-error
    }

    foreach($localDeployTarget in $localDeployTargets) { 

        $addinsPath = (join-path $localDeployTarget "addins");

        if (-not (test-path $addinsPath)) {
            mkdir $addinsPath
        }

        $filesToDeploy | % {
            cp (join-path $deploySource $_) $addinsPath
        }
    }
}

function RunNUnit {

    param ($dll, $run = "")

    $command = @( $nunit_path, $testDll)
    
    if ($run) {
        $command += /run:$run
    }

    $command = [String]::Join(" ", $command)

    exec { invoke-expression $command }
    
}

task UnitTest {

    RunNUnit $testDll
}

task IntegrationTest {

    $tests = @(
        @{
            test = "NJasmineTests.FailingFixtures.beforeEach_after_it";
            expectedSubstrings = @(
                "Test Failure : NJasmineTests.FailingFixtures.beforeEach_after_it", 
                "Exception thrown within test definition: Called beforeEach() after disposing().")
        },
        @{
            test = "NJasmineTests.FailingFixtures.ExceptionThrownAtTopLevel";
            expectedSubstrings = @(
                "Test Failure : NJasmineTests.FailingFixtures.ExceptionThrownAtTopLevel", 
                "Exception thrown within test definition: Attempted to divide by zero.")
        },
        @{
            test = "NJasmineTests.FailingFixtures.ExceptionThrownInFirstDescribe";
            expectedSubstrings = @(
                "Test Failure : ExceptionThrownInFirstDescribe.broken describe", 
                "Exception thrown within test definition: Attempted to divide by zero.");
        },
        @{
            test = "NJasmineTests.FailingFixtures.ReenterDuringAfterEach";
            expectedSubstrings = @(
                "Test Error : NJasmineTests.FailingFixtures.ReenterDuringAfterEach", 
                "System.InvalidOperationException : Called it() within afterEach().");
        },
        @{
            test = "NJasmineTests.FailingFixtures.ReentersDuringBeforeEach";
            expectedSubstrings = @(
                "Test Error : NJasmineTests.FailingFixtures.ReentersDuringBeforeEach",
                "System.InvalidOperationException : Called it() within beforeEach().")
        },
        @{
            test = "NJasmineTests.FailingFixtures.ReentersDuringIt";
            expectedSubstrings = @(
                "Test Error : NJasmineTests.FailingFixtures.ReentersDuringIt",
                "System.InvalidOperationException : Called it() within it().");
        },
        @{ 
            test = "NJasmineTests.Integration.imports_NUnit_fixture";
            succeeds = $true;
            expectedExtraction = @"
test started, before include of a
after include of a
first describe, before include of b
after include of b
before include of c
after include of c
FixtureSetup some_Nunit_fixture_a
FixtureSetup some_Nunit_fixture_b
FixtureSetup some_Nunit_fixture_c
test started, before include of a
SetUp some_Nunit_fixture_a
after include of a
first describe, before include of b
SetUp some_Nunit_fixture_b
after include of b
before include of c
SetUp some_Nunit_fixture_c
after include of c
second test test
TearDown some_Nunit_fixture_c
TearDown some_Nunit_fixture_b
TearDown some_Nunit_fixture_a
FixtureTearDown some_Nunit_fixture_c
test started, before include of a
SetUp some_Nunit_fixture_a
after include of a
first describe, before include of b
SetUp some_Nunit_fixture_b
after include of b
first test
TearDown some_Nunit_fixture_b
TearDown some_Nunit_fixture_a
FixtureTearDown some_Nunit_fixture_b
FixtureTearDown some_Nunit_fixture_a
"@
        },

        @{ 
            test = "NJasmineTests.Integration.suite_using_disposables";
            succeeds = $true;
            expectedExtraction = @"
test started, before include of a
after include of a
first describe, before include of b
after include of b
before include of c
after include of c
creating some_observable_A
creating some_observable_B
creating some_observable_C
disposingsome_observable_C
disposingsome_observable_B
disposingsome_observable_A
creating some_observable_A
creating some_observable_B
creating some_observable_D
creating some_observable_E
creating some_observable_F
disposingsome_observable_F
disposingsome_observable_E
disposingsome_observable_D
disposingsome_observable_B
disposingsome_observable_A
creating some_observable_A
creating some_observable_B
creating some_observable_D
disposingsome_observable_D
disposingsome_observable_B
disposingsome_observable_A
"@
        }
    )

    $tests | % { 

        $test = $_.test;

        "Running integration test $test." | write-host

        if ($_.succeeds) {
	        $testoutput = exec { & $nunit_path $testDll /run:$test }
        } else {
	        $testoutput = & $nunit_path $testDll /run:$test
        }

        $hasExpectation = $false;

        if ($_.expectedExtraction) {
            $expectedExtraction = $_.expectedExtraction.Split("`n") | % { $_.Trim() } | ? { -not $_.length -eq 0 }
            $actual = switch -r ($testoutput) { "<<{{(.*)}}>>" { $matches[1] } }
            $comparison = compare-object $expectedExtraction $actual
            if ($comparison) {
                $global:expected = $expectedExtraction;
                $global:actual = $actual;
                write-error "Unexpected results for `"$test`".  Expected written to `$global:expected, actual written to `$global:actual."
            }

            $hasExpectation = $true;
        } 
        
        $_.expectedSubstrings | % {

            if (-not [String]::Join("\n", $testoutput).Contains($_)) {
                $global:expected = $_;
                $global:actual = $testoutput;
                write-error "Unexpected results for `"$test`".  Expected written to `$global:expected, actual written to `$global:actual."
            }

            $hasExpectation = $true;
        }
        
        if (-not $hasExpectation) {
            "Test Skipped: No expectation found for $test" | write-host
        }
    }
}

task AllTests -depends Build, TestDeploy, UnitTest, IntegrationTest {
}
