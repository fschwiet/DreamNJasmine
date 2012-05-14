# 
# Copyright (c) 2011-2012, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 

properties {
  Write-Output "Loading nunit properties"
  $nunit = @{}
  $nunit.runner = (Get-ChildItem "$($base.dir)\*" -recurse -include nunit-console-x86.exe).FullName
}

function Invoke-TestRunner {
  param(
    [Parameter(Position=0,Mandatory=$true)]
    [string[]]$dlls = @()
  )

  Assert ((Test-Path($nunit.runner)) -and (![string]::IsNullOrEmpty($nunit.runner))) "NUnit runner could not be found"
  
  if ($dlls.Length -le 0) { 
     Write-Output "No tests defined"
     return 
  }
  exec { & $nunit.runner $dlls /noshadow /xml=$nunit.XmlTarget }
}

function Invoke-TestRunnerGui {
  param(
    [Parameter(Position=0,Mandatory=$true)]
    [string[]]$dlls = @()
  )

  Assert ((Test-Path($nunit.gui)) -and (![string]::IsNullOrEmpty($nunit.gui))) "NUnit GUI runner could not be found"
  
  if ($dlls.Length -le 0) { 
     Write-Output "No tests defined"
     return 
  }
  & $nunit.gui $dlls
}