
param($installPath, $toolsPath, $package, $project)

$nunitPaths = gci (join-path $installPath "..") "NUnit.$NUnitVersion$" | % { $_.fullname }
foreach($nunitPath in $nunitPaths) {
    $null = mkdir "$nunitPath\tools\addins"
    cp "$installPath\lib\*" "$nunitPath\tools\addins"
}