
properties {
    $base_dir  = resolve-path .
    $build_dir = "$base_dir\build\"
    $solution = "$base_dir\NJasmine.sln"
    $msbuild_Configuration = "Debug"
	$deploySource = "$base_dir\NJasmine\bin\$msbuild_Configuration\"
	$testDeployTarget = "$base_dir\packages\NUnit.2.5.7.10213\Tools\bin\"
	$testDll = "$base_dir\NJasmine.Tests\bin\$msbuild_Configuration\NJasmine.Tests.dll"
}

task default -depends Test

task Build {
	exec { & $msbuild $sln /property:Configuration=$msbuild_Configuration }
}

task Test -depends Build {

	rm $testDeployTarget -recurse
	mkdir $testDeployTarget
	cp $deploySource $testDeployTarget\addins -recurse

	exec { .\packages\NUnit.2.5.7.10213\Tools\nunit-console.exe $testDll }
}
