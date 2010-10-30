
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

task TestFixture_imports_NUnit_fixture {

    $tests = @(
        @{ 
            test = "NJasmineTests.Core.imports_NUnit_fixture";
            expected = @(
                "test started, before first include of a",
                "after first include of a",
                "first describe, before include of b",
                "after include of b",
                "before second a",
                "after second a",
                "FixtureSetup some_Nunit_fixture_a",
                "FixtureSetup some_Nunit_fixture_b",
                "FixtureSetup some_Nunit_fixture_a",
                "test started, before first include of a",
                "Setup some_Nunit_fixture_a",
                "after first include of a",
                "first describe, before include of b",
                "Setup some_Nunit_fixture_b",
                "after include of b",
                "before second a",
                "Setup some_Nunit_fixture_a",
                "after second a",
                "second test test",
                "test started, before first include of a",
                "after first include of a",
                "first describe, before include of b",
                "after include of b",
                "first test")
        }
    )

    $tests | % { 

        $test = $_.test;
        $expected = $_.expected;
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

task Test -depends Build {

	#exec { & $testDll }

    RunNUnit $testDll

}

