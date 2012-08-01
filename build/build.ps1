 
# Copyright (c) 2011-2012, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 

# The global settings are provided in the settings.ps1 file. If you
# want to override anything in it, you can use the overrides.ps1 to 
# replace any property value used in any indcluded file. Remember that 
# values provided on the command line will override all of these files below.
Include settings.ps1
#Include xunit.ps1
Include nunit.ps1
#Include nuget.ps1
Include msbuild.ps1
Include assemblyinfo.ps1
Include overrides.ps1
#Include git.ps1

Include psake_ext.ps1

properties {
  Write-Output "Loading build properties"
  # Do not put any code in here. This method is invoked before all others
  # and will not have access to any of your shared properties.
}

task Build -depends RunMSBuild, CopyNUnitToBuild, CopyVS2012TestToBuild
task Default -depends Initialize, TraceSourceControlCommit, Build, Test, IntegrationTest, BuildNuget
task RunGUI -depends KillNUnit, Build, RunNUnitGUI

Task Test { 
  Invoke-TestRunner @("$($build.dir)\NJasmine.tests.dll")
}

function VisitTests($testHandler) {

  $tests = ([xml](exec {& "$($build.dir)\NJasmine.TestUtil.exe" "list-tests"})).ArrayOfTestDefinition.TestDefinition | 
        ? { $_.Name -like $integrationTestRunPattern };

  foreach($test in $tests)  { 
  
    & $testHandler $test
  }
}

Task VisualStudioIntegrationTest { 
  
  try
  {
	$vsTestExe = "$($build.dir)\VS2012\vstest.console.exe"

	$discovererListing = exec {& $vsTestExe /ListDiscoverers }

	Assert ($discovererListing -match "NJasmine\.VS2012") "Expect to see NJasmine.VS2012 deployed for vstest.console.exe"

    VisitTests { 
      
      param ($test);

      $testName = $test.Name;
      $testDll = (resolve-path (join-path $build.dir "NJasmine.Tests.dll")).path
      
	  write-output "Checking $testName at $testDll."

      & $vsTestExe $testDll "/Tests:`"$testName`""
        #exec { & $vsTestExe (join-path $build.dir "NJasmine.Tests.dll") "/Tests:`"$testName`"" }
    }
  } finally {
    $Host.UI.RawUI.ForegroundColor= [ConsoleColor]::Gray
  }
}

Task NUnitIntegrationTest { 
  
  VisitTests { 
  
	param ($test);
	
    $testName = $test.Name;

    write-output "Checking $testName."

    $testXmlTarget = (join-path $build.dir "IntegrationTest.xml")
    $testConsoleTarget = (join-path $build.dir "IntegrationTest.txt")

    & (& get-nunit-console) "$($build.dir)\NJasmine.tests.dll" /run=$testName /xml=$testXmlTarget > $testConsoleTarget

    exec { & "$($build.dir)\NJasmine.TestUtil.exe" "verify-test" """$testName""" """$testXmlTarget""" """$testConsoleTarget""" }
  }
}

task IntegrationTest -depends VisualStudioIntegrationTest, NUnitIntegrationTest

Task Initialize -Depends Clean {
  New-Item $release.dir -ItemType Directory | Out-Null
  New-Item $build.dir -ItemType Directory | Out-Null
}

Task Clean { 
  Remove-Item -Force -Recurse $build.dir -ErrorAction SilentlyContinue | Out-Null
  Remove-Item -Force -Recurse $release.dir -ErrorAction SilentlyContinue | Out-Null
  if (gci $base.dir "obj.build" -rec) {
    #gci $base.dir "obj.build" -rec | rm  -recurse  #fails in VS Nuget console...
    foreach($dir in (gci $base.dir "obj.build" -rec)) {
      $dir | rm -rec | out-null
    }
  }
}

Task ? -Description "Helper to display task info" {
  Write-Documentation
}

task TraceSourceControlCommit {
  git log -1 --oneline | % { "Current commit: " + $_ }
}

task GenerateAssemblyInfo {
  $projectFiles = ls -path $base.dir -include *.csproj -recurse

  $projectFiles | write-host
  foreach($projectFile in $projectFiles) {
    $projectDir = [System.IO.Path]::GetDirectoryName($projectFile)
    $projectName = [System.IO.Path]::GetFileName($projectDir)
    $asmInfo = [System.IO.Path]::Combine($projectDir, [System.IO.Path]::Combine("Properties", "AssemblyInfo.cs"))
        
    Generate-Assembly-Info `
      -file $asmInfo `
      -title "$projectName $($build.version).0" `
      -description $shortDescription `
      -company "n/a" `
      -product "NJasmine $($build.version).0" `
      -version "$($build.version).0" `
      -fileversion "$($build.version).0" `
      -copyright "Copyright © Frank Schwieterman 2010-2011" `
      -clsCompliant "false"
  }
}

task RunMSBuild -depends Clean, Initialize, GenerateAssemblyInfo, Invoke-MsBuild {
  cp "$($base.dir)\getting started.txt" "$($build.dir)"
  cp "$($base.dir)\license.txt" "$($build.dir)\license-NJasmine.txt"
  cp "$($base.dir)\lib\PowerAssert\license-PowerAssert.txt" "$($build.dir)"
}

task CopyNUnitToBuild {
  $requiredNUnitFiles = @("nunit-agent.exe*", "nunit-console.exe*", "nunit.exe*", "lib", "nunit.framework.dll");
  $targetForNUnitFiles = (join-path $build.dir "nunit\")
  $targetForNUnitAddins = (join-path $targetForNUnitFiles "addins\")
 
  mkdir $targetForNUnitFiles | out-null
  mkdir $targetForNUnitAddins | out-null

  $NUnitLicensePath = (join-path $NUnitBinPath "..\license.txt");
  if (-not (test-path $NUnitLicensePath)) {
    $NUnitLicensePath = (join-path $NUnitBinPath "..\..\license.txt");
  }
  cp $NUnitLicensePath (join-path $build.dir "license-NUnit.txt")

  foreach($required in $requiredNUnitFiles) {
    cp (join-path $NUnitBinPath $required) $targetForNUnitFiles -rec
  }

  cp (join-path $build.dir "njasmine.dll") $targetForNUnitAddins
  cp (join-path $build.dir "njasmine.nunit.dll") $targetForNUnitAddins
  cp (join-path $build.dir "powerassert.dll") $targetForNUnitAddins
}

task KillNUnit {
  (ps nunit*) | % { $_.kill() }
}

task RunNUnitGUI {
  Invoke-TestRunnerGui @("$($build.dir)\NJasmine.tests.dll")
}

task CopyVS2012TestToBuild {

  $targetPath = (join-path $build.dir "VS2012")

  if (test-path $targetPath) {
    rm -force -recurse $targetPath
  }

  $null = mkdir $targetPath;
  cp -recurse $VS2012BinPath\* $targetPath

  $targetForExtensions = "$targetPath\Extensions"

  cp (join-path $build.dir "njasmine.dll") $targetForExtensions
  cp (join-path $build.dir "njasmine.vs2012.dll") $targetForExtensions
  cp (join-path $build.dir "powerassert.dll") $targetForExtensions
}

task BuildNuget {
  
  $nugetBuildPath = "$($build.dir)\nuget"
  
  if (test-path $nugetBuildPath) {
	rm $nugetBuildPath -force -rec
  }
  
  $nugetTargetLib = "$nugetBuildPath\NJasmine"
  $nugetTargetRunner = "$nugetBuildPath\NJasmine.NUnit"

  mkdir "$nugetTargetLib\lib\" | out-null
  mkdir "$nugetTargetLib\tools\" | out-null
  mkdir "$nugetTargetRunner\lib\" | out-null
  mkdir "$nugetTargetRunner\tools\" | out-null

  $packagesConfig = "$($base.dir)\NJasmine.NUnit\packages.config"
  $packagesConfigXml = [xml] (get-content $packagesConfig)
  $nunitVersion = @($packagesConfigXml.packages.package | ? { $_.id -eq "NUnit" } | % { $_.version })
  
  Assert ($nunitVersion.length -eq 1) "Expected to find NUnit version in $packagesConfig, found $($nunitVersion.length)."

  cp "$($base.dir)\NJasmine\NJasmine.nuspec" "$nugetTargetLib\"
  cp "$($build.dir)\NJasmine.dll" "$nugetTargetLib\lib\"
  cp "$($build.dir)\NJasmine.pdb" "$nugetTargetLib\lib\"
  cp "$($base.dir)\NJasmine.NUnit\NJasmine.NUnit.nuspec" "$nugetTargetRunner\"
  cp "$($build.dir)\NJasmine.dll" "$nugetTargetRunner\lib\"
  cp "$($build.dir)\NJasmine.pdb" "$nugetTargetRunner\lib\"
  cp "$($build.dir)\NJasmine.NUnit.dll" "$nugetTargetRunner\lib\"
  cp "$($build.dir)\NJasmine.NUnit.pdb" "$nugetTargetRunner\lib\"

  "Nuget package NJasmine is included to write tests, include NJasmine.NUnit to run tests http://nuget.org/packages/NJasmine.NUnit." | set-content "$nugetTargetLib\tools\readme.txt" -encoding utf8
  "Run NJasmine tests by running the NUnit runners included with NUnit.Runners.  Nuget package NJasmine.NUnit copies the necessary DLLs to the corresponding addins folder." | set-content "$nugetTargetRunner\tools\readme.txt" -encoding utf8

  (get-content "$($base.dir)\nuget.install.ps1") -replace "!NUnitVersion!",$nunitVersion | set-content "$nugetTargetRunner\tools\install.ps1" -encoding UTF8

  $packageVersion = "$($build.version).0"
  
  if ($prerelease) {
	$packageVersion = $packageVersion + "-beta"
  }

  update-xml "$nugetTargetLib\NJasmine.nuspec" {
    set-xml -exactlyOnce "//package/metadata/version" "$packageVersion"
  }

  update-xml "$nugetTargetRunner\NJasmine.NUnit.nuspec" {
    set-xml -exactlyOnce "//package/metadata/version" "$packageVersion"
    for-xml -exactlyOnce "//package/metadata/dependencies/dependency[@id='NJasmine']" {
	  set-attribute "version" $packageVersion
	}
  }
  
  exec { & "$($base.dir)\tools\NuGet.exe" pack "$nugetTargetLib\NJasmine.nuspec" -output $build.dir }
  exec { & "$($base.dir)\tools\NuGet.exe" pack "$nugetTargetRunner\NJasmine.NUnit.nuspec" -output $build.dir }
}

function get-nunit-console {
    (join-path $build.dir "nunit\nunit-console.exe")
}
