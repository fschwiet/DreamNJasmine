using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using NJasmine;
using NJasmine.Extras;

namespace NJasmineTests.Specs.AppDomains_like_a_boss
{
    public class Should_be_able_to_load_AppDomain : GivenWhenThenFixture
    {
        public override void Specify()
        {
            var someDllPath = arrange(() => GetSomeDllPath());

            expect(() => File.Exists(someDllPath));

            given("an AppDomain wrapper for a test DLL", delegate()
            {
                var appDomainWrapper = arrange(() => new AppDomainWrapper(someDllPath));

                then("the executor class can be loaded", () =>
                {
                    var o = appDomainWrapper.CreateObject<NJasmine.Marshalled.Executor>("NJasmine.dll");
                });

                then("the test DLL's configuration file is loaded", () =>
                {
                    var o = appDomainWrapper.CreateObject<NJasmine.Marshalled.Executor.AppSettingLoader>("NJasmine.dll");

                    string result = o.Get("someConfigurationValue");

                    expect(() => result == "#winning");
                });

                then("test DLL's tests can be enumerated", () =>
                {
                    Console.WriteLine("using " + someDllPath);

                    var o = appDomainWrapper.CreateObject<NJasmine.Marshalled.Executor.SpecEnumerator>("NJasmine.dll");

                    var result = o.GetTestNames(AssemblyName.GetAssemblyName(someDllPath).FullName);

                    expect(() => result.Contains("SomeTestLibrary.ASingleTest, first test"));
                });
            });
        }

        private string GetSomeDllPath()
        {
            var currentDllDirectory = new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath).Directory;

            if (currentDllDirectory.Name == "build")
                return Path.Combine(currentDllDirectory.FullName, "SomeTestLibrary.dll");
            else
                return Path.Combine(currentDllDirectory.FullName, "..\\..\\..\\SomeTestLibrary\\bin\\debug\\SomeTestLibrary.dll");
        }
    }

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
