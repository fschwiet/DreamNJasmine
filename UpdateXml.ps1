
param(
    [string] $xmlFile = $(throw "config is required"),
    [ScriptBlock] $action
)
 
$doc = New-Object System.Xml.XmlDocument
$doc.Load($xmlFile)
 
function Display([string] $xpath, [bool] $onlyIf = $true) {
    
    if ($onlyIf -eq $true) {
        $nodes = $doc.SelectNodes($xpath)
         
        foreach ($node in $nodes) {
            if ($node -ne $null) {
                if ($node.NodeType -eq "Element") {
                    $node.InnerXml | write-host
                }
                else {
                    $node.Value | write-host
                }
            }
        }
    }
}


function Config([string] $xpath, [string] $value, [bool] $onlyIf = $true) {
    
    if ($onlyIf -eq $true) {
        $nodes = $doc.SelectNodes($xpath)
         
        foreach ($node in $nodes) {
            if ($node -ne $null) {
                if ($node.NodeType -eq "Element") {
                    $node.InnerXml = $value
                }
                else {
                    $node.Value = $value
                }
            }
        }
    }
}
 
function Replace([string] $xpath, [string] $value, [bool] $onlyIf = $true) {
    
    if ($onlyIf -eq $true) {
        $nodes = $doc.SelectNodes($xpath)
        $newNodes = New-Object System.Xml.XmlDocument;
        $newNodes.LoadXml($value);
        foreach ($node in $nodes) {
            if ($node -ne $null) {
                $node.RemoveAll();
                $importNode = $doc.ImportNode($newNodes.DocumentElement, $true);
                $newNode = $node.ParentNode.ReplaceChild($importNode, $node);
            }
        }
    }
}
 
function Remove([string] $xpath, [bool] $onlyIf = $true) {
    if ($onlyIf -eq $true) {
        $nodes = $doc.SelectNodes($xpath)
         
        foreach($node in $nodes) {
            $nav = $node.CreateNavigator();
            $nav.DeleteSelf();
        }
    }
}

& $action
 
$doc.Save($xmlFile)