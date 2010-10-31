
properties {
    $base_dir  = resolve-path .
    $build_dir = "$base_dir\build\"
    $solution = "$base_dir\NJasmine.sln"
    $msbuild_Configuration = "Debug"
	$deploySource = "$base_dir\NJasmine\bin\$msbuild_Configuration\"
	$testDeployTarget = "$base_dir\packages\NUnit.2.5.7.10213\Tools\addins\"
	$testDll = "$base_dir\NJasmine.Tests\bin\$msbuild_Configuration\NJasmine.Tests.dll"
    $nunit_path = "$base_dir\packages\NUnit.2.5.7.10213\tools\nunit-console.exe"
}

task default -depends Test

task Build {
	exec { & $msbuild $sln /property:Configuration=$msbuild_Configuration }
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

task TestDeploy -depends Build {

	if (test-path $testDeployTarget) {
		rm $testDeployTarget -recurse
	}
	mkdir $testDeployTarget
	cp $deploySource\* $testDeployTarget -recurse
}

task IntegrationTests {

    $tests = @(
        @{ 
            test = "NJasmineTests.Core.imports_NUnit_fixture";
            expected = @"
test started, before include of a
after include of a
first describe, before include of b
after include of b
before include of c
after include of c
FixtureSetup NJasmineTests.Core.some_Nunit_fixture_a
FixtureSetup NJasmineTests.Core.some_Nunit_fixture_b
FixtureSetup NJasmineTests.Core.some_Nunit_fixture_c
test started, before include of a
SetUp NJasmineTests.Core.some_Nunit_fixture_a
after include of a
first describe, before include of b
SetUp NJasmineTests.Core.some_Nunit_fixture_b
after include of b
before include of c
SetUp NJasmineTests.Core.some_Nunit_fixture_c
after include of c
second test test
TearDown NJasmineTests.Core.some_Nunit_fixture_c
TearDown NJasmineTests.Core.some_Nunit_fixture_b
TearDown NJasmineTests.Core.some_Nunit_fixture_a
FixtureTearDown NJasmineTests.Core.some_Nunit_fixture_c
test started, before include of a
SetUp NJasmineTests.Core.some_Nunit_fixture_a
after include of a
first describe, before include of b
SetUp NJasmineTests.Core.some_Nunit_fixture_b
after include of b
first test
TearDown NJasmineTests.Core.some_Nunit_fixture_b
TearDown NJasmineTests.Core.some_Nunit_fixture_a
FixtureTearDown NJasmineTests.Core.some_Nunit_fixture_b
FixtureTearDown NJasmineTests.Core.some_Nunit_fixture_a
"@
        }
    )

    $tests | % { 

        $test = $_.test;

        "Running integration test $test." | write-host

        $expected = $_.expected.Split("`n") | % { $_.Trim() } | ? { -not $_.length -eq 0 }
	    $testoutput = exec { & $nunit_path $testDll /run:NJasmineTests.Core.imports_NUnit_fixture }
        $actual = switch -r ($testoutput) { "<<{{(.*)}}>>" { $matches[1] } }
        $comparison = compare-object $expected $actual
        if ($comparison) {
            $global:expected = $expected;
            $global:actual = $actual;
            write-error "Unexpected results for `"$test`".  Expected written to `$global:expected, actual written to `$global:actual."
        }
    }
}

task UnitTest -depends Build {

    RunNUnit $testDll
}

task Test -depends UnitTest, IntegrationTests {
}