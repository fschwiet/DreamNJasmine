# 
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

Task Default -depends Initialize, TraceSourceControlCommit, Compile
Task Release -depends Default, Package
Task Deploy -depends Publish
Task Compile -Depends Initialize, AllTests
task AllTests -depends Build, CopyNUnitToBuild, IntegrationTest

task RunGUI -depends KillNUnit, TraceSourceControlCommit, Build, CopyNUnitToBuild, RunNUnitGUI

Task Test { 
  Invoke-TestRunner @("$($build.dir)\NJasmine.tests.dll")
}

Task IntegrationTest -Depends Test { 
  $testResults = @();

  $tests = ([xml](exec {& "$($build.dir)\NJasmine.TestUtil.exe" "list-tests"})).ArrayOfTestDefinition.TestDefinition | 
        ? { $_.Name -like $integrationTestRunPattern };

  $global:t = $tests; # todo: is this needed any longer?

  foreach($test in $tests)  { 
    $testName = $test.Name;

    write-output "Checking $testName."

    $testXmlTarget = (join-path $build.dir "IntegrationTest.xml")
    $testConsoleTarget = (join-path $build.dir "IntegrationTest.txt")

    & (& get-nunit-console) "$($build.dir)\NJasmine.tests.dll" /run=$testName /xml=$testXmlTarget > $testConsoleTarget

    exec { & "$($build.dir)\NJasmine.TestUtil.exe" "verify-test" """$testName""" """$testXmlTarget""" """$testConsoleTarget""" }
  }
}

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

task Build -depends Clean, Initialize, GenerateAssemblyInfo, Invoke-MsBuild {
  cp "$($base.dir)\getting started.txt" "$($build.dir)"
  cp "$($base.dir)\license.txt" "$($build.dir)\license-NJasmine.txt"
  cp "$($base.dir)\lib\PowerAssert\license-PowerAssert.txt" "$($build.dir)"
}

task CopyNUnitToBuild -depends Build {
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

task BuildNuget -depends AllTests {
  $version = "$($build.version).0"
  $build = "$($base.dir)\build_2_6_0"
  $nugetTargetLib = "$($base.dir)\build_2_6_0\nuget\NJasmine"
  $nugetTargetRunner = "$($base.dir)\build_2_6_0\nuget\NJasmine.NUnit"

  mkdir "$nugetTargetLib\lib\" | out-null
  mkdir "$nugetTargetRunner\lib\" | out-null
  mkdir "$nugetTargetRunner\tools\" | out-null

  cp "$($base.dir)\NJasmine\NJasmine.nuspec" "$nugetTargetLib\"
  cp "$build\NJasmine.dll" "$nugetTargetLib\lib\"
  cp "$build\NJasmine.pdb" "$nugetTargetLib\lib\"
  cp "$($base.dir)\NJasmine.NUnit\NJasmine.NUnit.nuspec" "$nugetTargetRunner\"
  cp "$build\NJasmine.NUnit.dll" "$nugetTargetRunner\lib\"
  cp "$build\NJasmine.NUnit.pdb" "$nugetTargetRunner\lib\"
  cp "$($base.dir)\nuget.install.ps1" "$nugetTargetRunner\tools\install.ps1"

  update-xml "$nugetTargetLib\NJasmine.nuspec" {
    set-xml -exactlyOnce "//package/metadata/version" "$version"
  }

  update-xml "$nugetTargetRunner\NJasmine.NUnit.nuspec" {
    set-xml -exactlyOnce "//package/metadata/version" "$version"
    for-xml -exactlyOnce "//package/metadata/dependencies/dependency[@id='NJasmine']" {
	  set-attribute "version" $version
	}
  }
  
  exec { & "$($base.dir)\tools\NuGet.exe" pack "$nugetTargetLib\NJasmine.nuspec" -output "$build" }
  exec { & "$($base.dir)\tools\NuGet.exe" pack "$nugetTargetRunner\NJasmine.NUnit.nuspec" -output "$build" }
}

function get-nunit-console {
    (join-path $build.dir "nunit\nunit-console.exe")
}
