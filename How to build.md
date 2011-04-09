
In order to build this project, you need to execute the psake build script from the PowerShell console.
You do this using the following command from the root project directory (putting the build result in .\build):

	.\psake.ps1 default.ps1
	
You may need to allow script execution by running the following command as adminstrator:

	Set-ExecutionPolicy unrestricted
	
	
If you want to deploy the addin to the local install of NUnit 2.5.x:

    .\psake.ps1 .\default.ps1 Deploy
	
With it installed locally, you still need to run NUnit from c:\program files(x86)\nunit*\bin\net-2.0\addins.  The 'Deploy' task is not well supported, you may need to copy from the build result to your addins directory.

To work, NJasmine needs to be in the path .\addins\ relative to the nunit executeable.