
In order to build this project, you need to execute the psake build script from the PowerShell console.
You do this using the following command from the root project directory (putting the build result in .\build):

	.\psake.ps1 default.ps1 ZipAll
or
	.\psake.ps1 default.ps1 ZipBuildNuget
	
You may need to allow script execution by running the following command as adminstrator:

	Set-ExecutionPolicy unrestricted

	
If you want to install the addin to a local install of NUnit 2.5.10, you can run

    .\psake.ps1 .\default.ps1 Install
	