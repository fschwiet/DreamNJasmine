param(
  [Parameter(Position=0,Mandatory=0)]
  [string]$buildFile = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)\build\build.ps1",
  [Parameter(Position=1,Mandatory=0)]
  [string[]]$taskList = @(),
  [Parameter(Position=2,Mandatory=0)]
  [string]$framework = '4.0',
  [Parameter(Position=3,Mandatory=0)]
  [switch]$docs = $false,
  [Parameter(Position=4,Mandatory=0)]
  [System.Collections.Hashtable]$parameters = @{},
  [Parameter(Position=5, Mandatory=0)]
  [System.Collections.Hashtable]$properties = @{},
  [Parameter(Position=6, Mandatory=0)]
  [alias("init")]
  [scriptblock]$initialization = {},
  [Parameter(Position=7, Mandatory=0)]
  [switch]$nologo = $false,
  [Parameter(Position=8, Mandatory=0)]
  [switch]$help = $false,
  [Parameter(Position=9, Mandatory=0)]
  [string]$scriptPath = $(Split-Path -parent $MyInvocation.MyCommand.path)
)

$scriptPath = (Split-Path -parent $MyInvocation.MyCommand.Definition)
$buildPath = (Resolve-Path $scriptPath\build)

# '[p]sake' is the same as 'psake' but $Error is not polluted
remove-module [p]sake
$psakeModule = @(Get-ChildItem $scriptPath\* -recurse -include psake.ps1)[0].FullName

if ($help) {
  Get-Help Invoke-psake -full
  return
}

if (-not(test-path $buildFile)) {
  $absoluteBuildFile = (join-path $scriptPath $buildFile)
  if (test-path $absoluteBuildFile) {
    $buildFile = $absoluteBuildFile
  }
} 

. $buildPath\bootstrap.ps1 $buildPath
. $psakemodule $buildFile $taskList $framework $docs $parameters $properties $initialization $nologo
