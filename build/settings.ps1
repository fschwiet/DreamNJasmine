# 
# Copyright (c) 2011-2012, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 

# This file contains the build-wide settings used by all scripts.
# The overrides.ps1 can be used for CI settings where conventions don't work.

properties {
  Write-Output "Loading settings properties"
  
  $base = @{}
  $base.dir = Resolve-Path .\..\
  
  $source = @{}
  $source.dir = "$($base.dir)\src"
  if(!(Test-Path($source.dir))) { $source.dir = "$($base.dir)\source" }
  if(!(Test-Path($source.dir))) { $source.dir = "$($base.dir)" }
  
  $build = @{}
  $build.dir = "$($base.dir)\bin"
  $build.configuration = "Debug"
  
  # BUILD_NUMBER is defined during CI builds. Make sure that this value
  # is changed if the CI system in use does not set this variable.
  # Make sure Semver versioning is used for the build number.
  $build.version = "0.3.0"
  if($env:BUILD_NUMBER) { $build.version = $env:BUILD_NUMBER } 
  $prerelease = $false;

  $tools = @{}
  $tools.dir = "$($base.dir)\tools"
  
  $lib = @{}
  $lib.dir = "$($base.dir)\lib"
  
  $solution = @{}
  $solution.name = "NJasmine"
  $solution.file = "$($base.dir)\$($solution.name).sln"
  
  $release = @{}
  $release.dir = "$($base.dir)\release"
  
  $packages = @{}
  $packages.name = "lib"
  $packages.dir = "$($base.dir)\$($packages.name)"

  $NUnitBinPath = "$($base.dir)\packages\NUnit.Runners.2.6.0.12051\tools\"
  $filesToDeploy = @("NJasmine.dll", "NJasmine.xml", "NJasmine.NUnit.dll", "NJasmine.NUnit.xml", "PowerAssert.dll", "nunit.framework.dll", "license-*.txt", "getting started.txt")

  $VS2012BinPath = "$($base.dir)\lib\TestWindow"

  $integrationTestRunPattern = "*"

  $shortDescription = "NJasmine allows writing RSpec-like specifications."
  $longDescription = "NJasmine is an NUnit extension for writing given/when/then style specifications or tests.  Like RSpec, NJasmine as a DSL uses nested anonymous methods to build up a specification."
}