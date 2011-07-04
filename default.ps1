
properties {
    $base_dir  = resolve-path .
    $buildDir = "$base_dir\build\"
    $zipsDir = "$base_dir\zips\"

    $NUnitLibPath = "$base_dir\lib\NUnit-2.5.9.10348\bin\net-2.0\lib"
    $NUnitFrameworkPath = "$base_dir\lib\NUnit-2.5.9.10348\bin\net-2.0\framework"
    $NUnitBinPath = "$base_dir\lib\NUnit-2.5.9.10348\bin\net-2.0"

    $solution = "$base_dir\NJasmine.sln"
    $msbuild_Configuration = "Debug"
    $filesToDeploy = @("NJasmine.dll", "NJasmine.pdb", "PowerAssert.dll")
    $integrationTestRunPattern = "*"
}

import-module .\tools\PSUpdateXml.psm1

task default -depends AllTests

task Build {

    if (test-path $buildDir) {
        rmdir $buildDir -recurse
    }
    
    if (gci $base_dir "obj.build" -rec) {
        
        #gci $base_dir "obj.build" -rec | rm  -recurse  #fails in VS Nuget console...

        foreach($dir in (gci $base_dir "obj.build" -rec)) {
            $dir | rm -rec
        }
    }

    $v4_net_version = (ls "$env:windir\Microsoft.NET\Framework\v4.0*").Name
    exec { &"C:\Windows\Microsoft.NET\Framework\$v4_net_version\MSBuild.exe" "$sln_file" /p:BaseIntermediateOutputPath="obj.build\" /p:OutDir="$buildDir" /p:Configuration="$msbuild_Configuration" /p:NUnitLibPath="$NUnitLibPath" /p:NUnitFrameworkPath="$NUnitFrameworkPath" }

    cp "$base_dir\getting started.txt" $buildDir
    cp "$base_dir\license.txt" "$buildDir\license-NJasmine.txt"
    cp "$base_dir\lib\PowerAssert\license-PowerAssert.txt" $buildDir
}

task CopyNUnitToBuild -depends Build {

    $requiredNUnitFiles = @("nunit-agent.exe*", "nunit-console.exe*", "nunit.exe*", "lib", "nunit.framework.dll");
    $targetForNUnitFiles = (join-path $buildDir "nunit\")
    $targetForNUnitAddins = (join-path $targetForNUnitFiles "addins\")

    $null = mkdir $targetForNUnitFiles
    $null = mkdir $targetForNUnitAddins

    $NUnitLicensePath = (join-path $NUnitBinPath "..\license.txt");
    if (-not (test-path $NUnitLicensePath)) {
        $NUnitLicensePath = (join-path $NUnitBinPath "..\..\license.txt");
    }

    cp $NUnitLicensePath (join-path $buildDir "license-NUnit.txt")

    foreach($required in $requiredNUnitFiles) {
        cp (join-path $NUnitBinPath $required) $targetForNUnitFiles -rec
    }

    cp (join-path $buildDir "njasmine.dll") $targetForNUnitAddins
    cp (join-path $buildDir "powerassert.dll") $targetForNUnitAddins
}

function get-nunit-console {
    (join-path $buildDir "nunit\nunit-console.exe")
}

task UnitTests {

    $testOutputTarget = (join-path $buildDir "UnitTests.xml")

    exec { & (& get-nunit-console) "$buildDir\NJasmine.tests.dll" /xml=$testOutputTarget}
}

task IntegrationTests {

    $testResults = @();

    $tests = ([xml](& "$buildDir\NJasine.TestLoader.exe")).ArrayOfTestDefinition.TestDefinition | ? { $_.Name -like $integrationTestRunPattern }

    $global:t = $tests;

    "If an integation test fails, `$global:actual should have the actual output.  There may be other output files mentioned in the error text." | write-host

    foreach($test in $tests)  { 

        $testName = $test.Name;
        $expectedStrings = $test.ExpectedStrings.string;

        "Running integration test $testName." | write-host
        $testResults = $testResults + "Running integration test $testName."

        $testOutputTarget = (join-path $buildDir "IntegrationTest.xml")

        if ($test.TestPasses -eq "true") {

            $testoutput = & (& get-nunit-console) "$buildDir\NJasmine.tests.dll" /run=$testName /xml=$testOutputTarget
            
            Assert ($lastexitcode -eq 0) "Expected test $testName to pass, actual exit code: $lastexitcode."

        } else {

            $testoutput = & (& get-nunit-console) "$buildDir\NJasmine.tests.dll" /run=$testName /xml=$testOutputTarget
            
            Assert ($lastexitcode -ne 0) "Expected test $testName to fail."
        }

        $hasExpectation = $false;

        if ($test.VerificationScript) {

            $verificationCommand = "{" + $test.VerificationScript + "}"
            $verificationCommand = invoke-expression $verificationCommand

            $global:actual = $testoutput
            $global:actualError = $testOutputTarget

            & $verificationCommand $testoutput $testOutputTarget

            $hasExpectation = $true;
        }

        if ($test.ExpectedExtraction) {
            $expectedExtraction = $test.ExpectedExtraction.Split("`n") | % { $_.Trim() } | ? { -not $_.length -eq 0 }

            $actual = @()
            
            switch -r ($testoutput) { "`{`{<<RESET>>`}`}" { $actual = @(); } "<<{{(.*)}}>>" { $actual += $matches[1] } }

            $comparison = compare-object $expectedExtraction $actual
            if ($comparison) {
                $global:expected = $expectedExtraction;
                $global:actual = $actual;
                $global:fullActual = $testoutput;
                $error = "Unexpected extraction results for `"$testName`".  Expected written to `$global:expected, actual written to `$global:actual and `$global:fullActual."
                write-error $error
                $testResults = $testResults + $error
            }

            $hasExpectation = $true;
        } 
        
        $expectedStrings | % {

            if (-not [String]::Join("\n", $testoutput).Contains($_)) {
                $global:expected = $_;
                $global:fullExpected = $expectedStrings;
                $global:actual = $testoutput;
                $error = "Unexpected contains results for `"$testName`".  Expected written to `$global:expected and `$global:fullExpected, actual written to `$global:actual."
                write-error $error
                $testResults = $testResults + $error
            }

            $hasExpectation = $true;
        }
        
        if ($test.ExpectedTestNames.length -gt 0) {

            $allExpected = $test.ExpectedTestNames | % { $_.string };

            $testResults = [xml] (get-content $testOutputTarget)

            $testNames = $testResults.SelectNodes("//test-results/descendant::test-case/@name") | % { $_."#text" }

            $global:actual = $testNames

            foreach($expectedTestName in $allExpected) {

                $global:expected = $expectedTestName;

                Assert ($testNames -contains $expectedTestName) "Did not find expected test name.  Expected name stored in `$global:expected, actual stored in `$global:actual."

                $hasExpectation = $true;
            }
        }

        if (-not $hasExpectation) {
            "Test Skipped: No expectation found for $testName" | write-host
        }   
    }
}

task AllTests -depends Build, CopyNUnitToBuild, UnitTests, IntegrationTests {
}

function TrackPackage($zipFile, $buildLocation) {

    if (-not $script:packages) {
        $script:packages = @{};
    }

    $script:packages[$zipFile] = $buildLocation
}

task Build_2_5_9 {

    $buildDir = "$base_dir\build_2_5_9\"
    $NUnitLibPath = "$base_dir\lib\NUnit-2.5.9.10348\bin\net-2.0\lib"
    $NUnitFrameworkPath = "$base_dir\\lib\NUnit-2.5.9.10348\bin\net-2.0\framework"
    $NUnitBinPath = "$base_dir\lib\NUnit-2.5.9.10348\bin\net-2.0"

    TrackPackage "NJasmine_for_NUnit-2.5.9.zip" $buildDir

    invoke-psake -buildFile default.ps1 -taskList @("AllTests") -properties @{ buildDir=$buildDir; NUnitLibPath=$NUnitLibPath; NUnitFrameworkPath=$NUnitFrameworkPath}    
}

task Build_2_5_10 {

    $buildDir = "$base_dir\build_2_5_10\"
    $NUnitLibPath = "$base_dir\lib\NUnit-2.5.10.11092\bin\net-2.0\lib"
    $NUnitFrameworkPath = "$base_dir\lib\NUnit-2.5.10.11092\bin\net-2.0\framework"
    $NUnitBinPath = "$base_dir\lib\NUnit-2.5.10.11092\bin\net-2.0"

    TrackPackage "NJasmine_for_NUnit-2.5.10 (stable).zip" $buildDir

    invoke-psake -buildFile default.ps1 -taskList @("AllTests") -properties @{ buildDir=$buildDir; NUnitLibPath=$NUnitLibPath; NUnitFrameworkPath=$NUnitFrameworkPath; NUnitBinPath=$NUnitBinPath}    
}

task Build_2_6_0 {

    $buildDir = "$base_dir\build_2_6_0\"
    $NUnitLibPath = "$base_dir\lib\NUnit-2.6.0.11089\bin\lib"
    $NUnitFrameworkPath = "$base_dir\lib\NUnit-2.6.0.11089\bin\framework"
    $NUnitBinPath = "$base_dir\lib\NUnit-2.6.0.11089\bin"

    TrackPackage "NJasmine_for_NUnit-2.6.0 (preview).zip" $buildDir

    invoke-psake -buildFile default.ps1 -taskList @("AllTests") -properties @{ buildDir=$buildDir; NUnitLibPath=$NUnitLibPath; NUnitFrameworkPath=$NUnitFrameworkPath; NUnitBinPath=$NUnitBinPath}    
}

task CleanZips {

    if (test-path $zipsDir) {
        rm $zipsDir -recurse
    }

    $null = mkdir $zipsDir
}

task ZipAll -depends CleanZips, Build_2_5_9, Build_2_5_10, Build_2_6_0 {

    $script:packages.GetEnumerator() | % {

        $zipFile = (join-path $zipsDir $_.Key)
        $buildResult = $_.Value;

        "packaging '$zipFile' from $buildResult"
        
        $filesToDeploy = @("NJasmine.dll", "NJasmine.xml", "PowerAssert.dll", "nunit.framework.dll", "license-*.txt", "getting started.txt") | % {
            (join-path $buildResult $_)
        }

        .\lib\7-Zip\7za.exe a $zipFile $filesToDeploy
    }
}

task Install -depends Build_2_5_10 {

    $target = "C:\Program Files\NUnit 2.5.10\bin";

    if (-not (test-path $target)) {
        $target = "C:\Program Files (x86)\NUnit 2.5.10\bin\net-2.0";
    }

    assert (test-path $target) "Install task could not find NUnit 2.5.10 installed."

    $target = (join-path $target "addins")

	if (-not (test-path $target)) {
		$null = mkdir $target
	}

    cp (join-path "$base_dir\build_2_5_10\" NJasmine.dll) $target
    cp (join-path "$base_dir\build_2_5_10\" PowerAssert.dll) $target
}

task BuildNuget -depends Build_2_5_10 {

    $build = "$base_dir\build_2_5_10"
    $nugetTarget = "$base_dir\build_2_5_10\nuget"

    $null = mkdir "$nugetTarget\lib\"

    cp "$build\NJasmine.dll" "$nugetTarget\lib\"
    cp "$build\NJasmine.pdb" "$nugetTarget\lib\"
    cp "$base_dir\lib\PackageDependencies\*" $build

    $old = pwd
    cd $nugetTarget

    ..\..\tools\nuget.exe spec -a ".\lib\NJasmine.dll"

    update-xml "NJasmine.nuspec" {

        add-xmlnamespace "ns" "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"

        set-xml -exactlyOnce "//ns:owners" "fschwiet"

        set-xml -exactlyOnce "//ns:licenseUrl" "https://github.com/fschwiet/DreamNJasmine/blob/master/LICENSE.txt"
        set-xml -exactlyOnce "//ns:projectUrl" "https://github.com/fschwiet/DreamNJasmine/"
        remove-xml -exactlyOnce "//ns:iconUrl"
        set-xml -exactlyOnce "//ns:tags" "BDD, NUnit"

        set-xml -exactlyOnce "//ns:dependencies" ""
        append-xml -exactlyOnce "//ns:dependencies" "<dependency id=`"NUnit`" version=`"2.5.10`" />"
        append-xml -exactlyOnce "//ns:dependencies" "<dependency id=`"PowerAssert`" version=`"1.0.2`" />"
    }

    ..\..\tools\nuget pack "NJasmine.nuspec"

    cd $old
}


