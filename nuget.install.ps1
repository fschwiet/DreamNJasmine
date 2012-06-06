
param($installPath, $toolsPath, $package, $project)

$nunitPaths = gci (join-path $installPath "..") "NUnit.Runners.!NUnitVersion!" | % { $_.fullname }

foreach($nunitPath in $nunitPaths) {
    
    $targetPath = "$nunitPath\tools\addins";
    
    if (-not (test-path $targetPath)) {
        $null = mkdir $targetPath
    }
    
    cp "$installPath\lib\*" "$nunitPath\tools\addins"
}