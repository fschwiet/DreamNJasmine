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

    write-output "Running integration test $testName."

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

task CleanZips {
  if (test-path $release.dir) {
    rm $release.dir -recurse
  }

  mkdir $release.dir | out-null
}

task Package -depends CleanZips, Build_2_5_9, Build_2_5_10, Build_2_6_0 {
  $script:packages.GetEnumerator() | % {
    $zipFile = (join-path $release.dir $_.Key)
    $buildResult = $_.Value;

    write-output "packaging '$zipFile' from $buildResult"
        
    $filesToDeploy | % {
      (join-path $buildResult $_)
    }

    & "$($lib.dir)\7-Zip\7za.exe" a $zipFile $filesToDeploy
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
      -title "$projectName $build.version.0" `
      -description $shortDescription `
      -company "n/a" `
      -product "NJasmine $build.version.0" `
      -version "$($build.version).0" `
      -fileversion "$($build.version).0" `
      -copyright "Copyright © Frank Schwieterman 2010-2011" `
      -clsCompliant "false"
  }
}

task Build -depends Clean, GenerateAssemblyInfo, Invoke-MsBuild {
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

function TrackPackage($zipFile, $buildLocation) {
  if (-not $script:packages) {
    $script:packages = @{};
  }

  $script:packages[$zipFile] = $buildLocation
}

task RunNUnitGUI {
  Invoke-TestRunnerGui @("$($build.dir)\NJasmine.tests.dll")
}

task Build_2_5_9 {
  $NUnitLibPath = "$($lib.dir)\NUnit-2.5.9.10348\bin\net-2.0\lib"
  $NUnitFrameworkPath = "$($lib.dir)\NUnit-2.5.9.10348\bin\net-2.0\framework"
  $NUnitBinPath = "$($lib.dir)\NUnit-2.5.9.10348\bin\net-2.0"

  TrackPackage "NJasmine_for_NUnit-2.5.9.zip" $build.dir
  
  invoke-psake -buildFile .\build.ps1 -taskList @("AllTests") -properties @{ NUnitLibPath=$NUnitLibPath; NUnitFrameworkPath=$NUnitFrameworkPath} -init { $build.dir = "$($base.dir)\build_2_5_9\" }
}

task Build_2_5_10 {
  $NUnitLibPath = "$($lib.dir)\NUnit-2.5.10.11092\bin\net-2.0\lib"
  $NUnitFrameworkPath = "$($lib.dir)\NUnit-2.5.10.11092\bin\net-2.0\framework"
  $NUnitBinPath = "$($lib.dir)\NUnit-2.5.10.11092\bin\net-2.0"

  TrackPackage "NJasmine_for_NUnit-2.5.10 (stable).zip" $build.dir
  
  invoke-psake -buildFile .\build.ps1 -taskList @("AllTests") -properties @{ NUnitLibPath=$NUnitLibPath; NUnitFrameworkPath=$NUnitFrameworkPath; NUnitBinPath=$NUnitBinPath}  -init { $build.dir = "$($base.dir)\build_2_5_10\" }
}

task Build_2_6_0 {
  $NUnitLibPath = "$($lib.dir)\NUnit-2.6.0.12051\bin\lib"
  $NUnitFrameworkPath = "$($lib.dir)\NUnit-2.6.0.12051\bin\framework"
  $NUnitBinPath = "$($lib.dir)\NUnit-2.6.0.12051\bin"

  TrackPackage "NJasmine_for_NUnit-2.6.0.zip" $build.dir

  invoke-psake -buildFile .\build.ps1 -taskList @("AllTests") -properties @{ NUnitLibPath=$NUnitLibPath; NUnitFrameworkPath=$NUnitFrameworkPath; NUnitBinPath=$NUnitBinPath} -init { $build.dir = "$($base.dir)\build_2_6_0\" }
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

  cp (join-path "$($base.dir)\build_2_5_10\" NJasmine.dll) $target
  cp (join-path "$($base.dir)\build_2_5_10\" PowerAssert.dll) $target
}

task BuildNuget -depends Build_2_5_10 {
  $version = "$($build.version).0"
  $build = "$($base.dir)\build_2_5_10"
  $nugetTarget = "$($base.dir)\build_2_5_10\nuget"

  mkdir "$nugetTarget\lib\" | out-null
  mkdir "$nugetTarget\tools\" | out-null

  cp "$build\NJasmine.dll" "$nugetTarget\lib\"
  cp "$build\NJasmine.pdb" "$nugetTarget\lib\"
  cp "$build\NJasmine.NUnit.dll" "$nugetTarget\lib\"
  cp "$build\NJasmine.NUnit.pdb" "$nugetTarget\lib\"
  cp "$($base.dir)\nuget.install.ps1" "$nugetTarget\tools\install.ps1"

  pushd $nugetTarget
  try {
    nuget.exe spec -a ".\lib\NJasmine.dll" 
	
    update-xml "NJasmine.nuspec" {
      #add-xmlnamespace "ns" "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"

      for-xml "//package/metadata" {
        set-xml -exactlyOnce "//version" "$version"
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

    nuget.exe pack "NJasmine.nuspec"

  } finally { popd }
}

function get-nunit-console {
    (join-path $build.dir "nunit\nunit-console.exe")
}