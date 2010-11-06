
properties {
    $base_dir  = resolve-path .

    # to build/test against another install of NUnit, override the following {{
    $wipeDeployTarget = $true
	$deployTarget = "$base_dir\packages\NUnit.2.5.7.10213\Tools\addins"
    $nunitBinPath = "$base_dir\packages\NUnit.2.5.7.10213\tools\nunit-console.exe"
    # }}

    $solution = "$base_dir\NJasmine.sln"
    $msbuild_Configuration = "Debug"
	$deploySource = "$base_dir\NJasmine\bin\$msbuild_Configuration\"
	$testDll = "$base_dir\NJasmine.Tests\bin\$msbuild_Configuration\NJasmine.Tests.dll"
    $filesToDeploy = @("NJasmine.dll", "NJasmine.pdb", "Should.Fluent.dll")
}

task default -depends AllTests

task Build {
    exec { & $msbuild $sln /property:Configuration=$msbuild_Configuration }
}

task Deploy -depends Build {

    "Deploying to $deployTarget." | write-host

	if ($wipeDeployTarget -and (test-path $deployTarget)) {
		rm $deployTarget -recurse
	}

    if (-not (test-path $deployTarget)) {
	    $null = mkdir $deployTarget
    }

	$filesToDeploy | % { cp (join-path $deploySource $_) $deployTarget -recurse -force }
}

task UnitTests {

    exec { & $nunitBinPath $testDll}
}

. .\IntegrationTests.ps1

task AllTests -depends Build, Deploy, UnitTests, IntegrationTests {
    "Ran NUnit at #nunitBinPath." | write-host
}

task BuildForInstalledNUnits {

    GetAllNUnits | % {
	    
        $rootPath = $_.rootPath;   
        $addinPath = $_.addinPath;
        $binPath = $_.binPath;
     
        try {

            SetAllProjectsToUseNUnitAt $rootPath
            
            invoke-psake -buildFile default.ps1 -taskList @("AllTests") -properties @{ deployTarget=$addinPath; nunitBinPath=$binPath; wipeDeployTarget=$false}
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

function SetAllProjectsToUseNUnitAt($path = "..\packages\NUnit.2.5.7.10213\") {

    $path = resolve-path $path

    (".\NJasmine\NJasmine.csproj", ".\NJasmine.Tests\NJasmine.Tests.csproj") | % {
        .\ForXml.ps1 (resolve-path $_) { 
            
            add-xmlnamespace "ns" "http://schemas.microsoft.com/developer/msbuild/2003";  
            
            @("nunit.core", "nunit.core.interfaces", "nunit.framework") | % {
                $dll = $_;
                $path | write-host
                (get-childitem $path ($dll + ".dll") -recurse).fullname | write-host
                $filepath = (get-childitem $path ($dll + ".dll") -recurse).fullname

                if (-not (test-path $filepath)) {
                    write-error "Unable to find $dll.dll for target NUnit deployment at $path"
                }

                set-xml "//ns:Reference[@Include='$dll']" "<HintPath>$filepath</HintPath>"
            }
        }
    }
}

