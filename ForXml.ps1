
param(
    [string] $xmlFile = $(throw "xmlFile is required"),
    [ScriptBlock] $action = $(throw "action is required")
)
 
$doc = New-Object System.Xml.XmlDocument
$nsmgr = New-Object System.Xml.XmlNamespaceManager $doc.NameTable

$doc.Load($xmlFile)
 
function add-xmlnamespace([string] $name, [string] $value) {
    $nsmgr.AddNamespace( $name, $value);
}

function get-xml([string] $xpath, [bool] $onlyIf = $true) {
    
    if ($onlyIf -eq $true) {
        $nodes = $doc.SelectNodes($xpath, $nsmgr)
         
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


function set-xml([string] $xpath, [string] $value, [bool] $onlyIf = $true) {
    
    if ($onlyIf -eq $true) {
        $nodes = $doc.SelectNodes($xpath, $nsmgr)
         
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
 
function replace-xml([string] $xpath, [string] $value, [bool] $onlyIf = $true) {
    
    if ($onlyIf -eq $true) {
        $nodes = $doc.SelectNodes($xpath, $nsmgr)
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