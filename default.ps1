
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

function SetAllProjectsToUseNUnitAt($path = "..\packages\NUnit.2.5.7.10213\") {
    (".\NJasmine\NJasmine.csproj", ".\NJasmine.Tests\NJasmine.Tests.csproj") | % {
        .\ForXml.ps1 (resolve-path $_) { 
            
            add-xmlnamespace "ns" "http://schemas.microsoft.com/developer/msbuild/2003";  
            
            (@("Tools\lib", "nunit.core"), @("Tools\lib", "nunit.core.interfaces"), @("lib", "nunit.framework")) | % {
                $dll = $_[1]
                $subpath = $_[0]
                $filepath = (join-path (join-path $path $subpath) $dll)
                set-xml "//ns:Reference[@Include='$dll']" "<HintPath>$filepath.dll</HintPath>"
            }
        }
    }
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

task UnitTests {

    RunNUnit $testDll
}

. .\IntegrationTests.ps1

task AllTests -depends Build, TestDeploy, UnitTests, IntegrationTests {
}
