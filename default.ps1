
properties {

    $version = "0.1.8"

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

    $shortDescription = "NJasmine allows writing RSpec-like specifications."
    $longDescription = "NJasmine is an NUnit extension for writing given/when/then style specifications or tests.  Like RSpec, NJasmine as a DSL uses nested anonymous methods to build up a specification."
}

. .\psake_ext.ps1
import-module .\tools\PSUpdateXml.psm1

task default -depends TraceSourceControlCommit,AllTests

task TraceSourceControlCommit {
    git log -1 --oneline | % { "Current commit: " + $_ }
}

task GenerateAssemblyInfo {
	
	$projectFiles = ls -path $base_dir -include *.csproj -recurse

    $projectFiles | write-host
	foreach($projectFile in $projectFiles) {
		
		$projectDir = [System.IO.Path]::GetDirectoryName($projectFile)
		$projectName = [System.IO.Path]::GetFileName($projectDir)
		$asmInfo = [System.IO.Path]::Combine($projectDir, [System.IO.Path]::Combine("Properties", "AssemblyInfo.cs"))
				
		Generate-Assembly-Info `
			-file $asmInfo `
			-title "$projectName $version.0" `
			-description $shortDescription `
			-company "n/a" `
			-product "NJasmine $version.0" `
			-version "$version.0" `
			-fileversion "$version.0" `
			-copyright "Copyright © Frank Schwieterman 2010-2011" `
			-clsCompliant "false"
	}
}

task Build -depends GenerateAssemblyInfo {

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

    $testXmlTarget = (join-path $buildDir "UnitTests.xml")

    exec { & (& get-nunit-console) "$buildDir\NJasmine.tests.dll" /xml=$testXmlTarget}
}

task IntegrationTests {

    $testResults = @();

    $tests = ([xml](exec {& "$buildDir\NJasmine.TestUtil.exe" "list-tests"})).ArrayOfTestDefinition.TestDefinition | 
        ? { $_.Name -like $integrationTestRunPattern };

    $global:t = $tests;

    foreach($test in $tests)  { 

        $testName = $test.Name;

        "Running integration test $testName." | write-host

        $testXmlTarget = (join-path $buildDir "IntegrationTest.xml")
        $testConsoleTarget = (join-path $buildDir "IntegrationTest.txt")

        & (& get-nunit-console) "$buildDir\NJasmine.tests.dll" /run=$testName /xml=$testXmlTarget > $testConsoleTarget

        exec { & "$buildDir\NJasmine.TestUtil.exe" "verify-test" """$testName""" """$testXmlTarget""" """$testConsoleTarget""" }
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
    $NUnitLibPath = "$base_dir\lib\NUnit-2.6.0.11240\bin\lib"
    $NUnitFrameworkPath = "$base_dir\lib\NUnit-2.6.0.11240\bin\framework"
    $NUnitBinPath = "$base_dir\lib\NUnit-2.6.0.11240\bin"

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
    $null = mkdir "$nugetTarget\tools\"

    cp "$build\NJasmine.dll" "$nugetTarget\lib\"
    cp "$build\NJasmine.pdb" "$nugetTarget\lib\"
    cp "$base_dir\nuget.install.ps1" "$nugetTarget\tools\install.ps1"

    $old = pwd
    cd $nugetTarget

    ..\..\tools\nuget.exe spec -a ".\lib\NJasmine.dll"

    update-xml "NJasmine.nuspec" {

        #add-xmlnamespace "ns" "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"

        for-xml "//package/metadata" {
            set-xml -exactlyOnce "//version" "$version.0"
            set-xml -exactlyOnce "//owners" "fschwiet"
            set-xml -exactlyOnce "//authors" "Frank Schwieterman"
            set-xml -exactlyOnce "//description" $longDescription

            set-xml -exactlyOnce "//licenseUrl" "https://github.com/fschwiet/DreamNJasmine/blob/master/LICENSE.txt"
            set-xml -exactlyOnce "//projectUrl" "https://github.com/fschwiet/DreamNJasmine/"
            remove-xml -exactlyOnce "//iconUrl"
            set-xml -exactlyOnce "//tags" "BDD TDD NUnit"

            set-xml -exactlyOnce "//dependencies" ""
            append-xml -exactlyOnce "//dependencies" "<dependency id=`"NUnit`" version=`"2.5.10`" />"
            append-xml -exactlyOnce "//dependencies" "<dependency id=`"PowerAssert`" version=`"1.0.2`" />"

            append-xml "." "<summary>$shortDescription</summary>"
        }
    }

    ..\..\tools\nuget pack "NJasmine.nuspec"

    cd $old
}


