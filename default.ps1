
properties {
    $base_dir  = resolve-path .
    $buildDir = "$base_dir\build\"

    $NUnitLibPath = "$base_dir\\lib\NUnit-2.5.9.10348\net-2.0\lib"
    $NUnitFrameworkPath = "$base_dir\\lib\NUnit-2.5.9.10348\net-2.0\framework"
    $NUnitBinPath = "$base_dir\lib\NUnit-2.5.9.10348\net-2.0"

    # to build/test against another install of NUnit, override the following {{
    $wipeDeployTarget = $true
    $deployTarget = "$base_dir\lib\NUnit-2.5.9.10348\net-2.0\addins"
    # }}

    $solution = "$base_dir\NJasmine.sln"
    $msbuild_Configuration = "Debug"
    $filesToDeploy = @("NJasmine.dll", "NJasmine.pdb", "PowerAssert.dll")
    $integrationTestRunPattern = "*"
}

task default -depends AllTests

task Build {

    if (test-path $buildDir) {
        rmdir $buildDir -recurse
    }

    $v4_net_version = (ls "$env:windir\Microsoft.NET\Framework\v4.0*").Name
    exec { &"C:\Windows\Microsoft.NET\Framework\$v4_net_version\MSBuild.exe" "$sln_file" /p:OutDir="$buildDir" /p:Configuration="$msbuild_Configuration" /p:NUnitLibPath="$NUnitLibPath" /p:NUnitFrameworkPath="$NUnitFrameworkPath" }
}

task CopyNUnitToBuild -depends Build {

    $requiredNUnitFiles = @("nunit-agent.exe*", "nunit-console.exe*", "nunit.exe*", "lib", "nunit.framework.dll");
    $targetForNUnitFiles = (join-path $buildDir "nunit\")
    $targetForNUnitAddins = (join-path $targetForNUnitFiles "addins\")

    $null = mkdir $targetForNUnitFiles
    $null = mkdir $targetForNUnitAddins

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

task Build_2_5_9 {

    $buildDir = "$base_dir\build_2_5_9\"
    $NUnitLibPath = "$base_dir\lib\NUnit-2.5.9.10348\net-2.0\lib"
    $NUnitFrameworkPath = "$base_dir\\lib\NUnit-2.5.9.10348\net-2.0\framework"
    $NUnitBinPath = "$base_dir\lib\NUnit-2.5.9.10348\net-2.0"
    invoke-psake -buildFile default.ps1 -taskList @("AllTests") -properties @{ buildDir=$buildDir; NUnitLibPath=$NUnitLibPath; NUnitFrameworkPath=$NUnitFrameworkPath}    
}

task Build_2_5_10 {

    $buildDir = "$base_dir\build_2_5_10\"
    $NUnitLibPath = "$base_dir\lib\NUnit-2.5.10.11092\bin\net-2.0\lib"
    $NUnitFrameworkPath = "$base_dir\lib\NUnit-2.5.10.11092\bin\net-2.0\framework"
    $NUnitBinPath = "$base_dir\lib\NUnit-2.5.10.11092\bin\net-2.0"
    invoke-psake -buildFile default.ps1 -taskList @("AllTests") -properties @{ buildDir=$buildDir; NUnitLibPath=$NUnitLibPath; NUnitFrameworkPath=$NUnitFrameworkPath; NUnitBinPath=$NUnitBinPath}    
}

task BuildAll -depends Build_2_5_9, Build_2_5_10 {
}

task Deploy {

    GetAllNUnits | % {
        
        $rootPath = $_.rootPath;   
        $addinPath = $_.addinPath;
        $binPath = $_.binPath;
     
        try {

            SetAllProjectsToUseNUnitAt $rootPath
            
            invoke-psake -buildFile default.ps1 -taskList @("AllTests") -properties @{ deployTarget=$addinPath; NUnitBinPath=$binPath; wipeDeployTarget=$false}
        } finally {
            SetAllProjectsToUseNUnitAt
        }
    }
}

function GetAllNUnits {
    (get-item 'C:\Program Files (x86)\NUnit 2.*\bin\net-2.0\framework') | 
    % { @{ 
        rootPath = (resolve-path (join-path $_ "..")); 
        addinPath = (join-path (resolve-path (join-path $_ "..")) "addins") 
        binPath = (resolve-path (join-path $_ "..\nunit-console.exe"))
    } };
}

function SetAllProjectsToUseNUnitAt($path = "..\lib\NUnit-2.5.9.10348\net-2.0\") {

    (".\NJasmine\NJasmine.csproj", ".\NJasmine.Tests\NJasmine.Tests.csproj") | % {
        .\ForXml.ps1 (resolve-path $_) { 
            
            add-xmlnamespace "ns" "http://schemas.microsoft.com/developer/msbuild/2003";  
            
            @("nunit.core", "nunit.core.interfaces", "nunit.framework") | % {
                $dll = $_;
                $filepath = @(get-childitem $path ($dll + ".dll") -recurse)[0].fullname

                if (-not (test-path $filepath)) {
                    write-error "Unable to find $dll.dll for target NUnit deployment at $path"
                }

                set-xml "//ns:Reference[@Include='$dll']" "<HintPath>$filepath</HintPath>"
            }
        }
    }
}

