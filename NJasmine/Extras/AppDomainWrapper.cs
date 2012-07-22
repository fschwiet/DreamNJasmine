using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace NJasmine.Extras
{
    public struct AppDomainWrapper
    {
        private string _dllPath;
        private AppDomain _domain;

        public AppDomainWrapper(string dllPath)
        {
            _dllPath = Path.GetFullPath(dllPath);

            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Path.GetDirectoryName(_dllPath);
            setup.ApplicationName = Guid.NewGuid().ToString();

            setup.ShadowCopyFiles = "true";
            setup.ShadowCopyDirectories = setup.ApplicationBase;
            setup.CachePath = Path.Combine(Path.GetTempPath(), setup.ApplicationName);

            var possibleConfigFile = _dllPath + ".config";
            setup.ConfigurationFile = File.Exists(possibleConfigFile) ? possibleConfigFile : null;

            _domain = AppDomain.CreateDomain(setup.ApplicationName, null, setup, new PermissionSet(PermissionState.Unrestricted));
        }

        public T CreateObject<T>(string dllName)
        {
            var assemblyName =
                AssemblyName.GetAssemblyName(Path.Combine(new FileInfo(_dllPath).Directory.FullName, dllName));

            return (T)_domain.CreateInstanceAndUnwrap(assemblyName.FullName, typeof(T).FullName);
        }
    }
}