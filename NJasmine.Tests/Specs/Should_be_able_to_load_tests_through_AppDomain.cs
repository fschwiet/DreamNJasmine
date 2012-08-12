using System;
using System.IO;
using System.Linq;
using Moq;
using NJasmine;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Extras;
using NJasmine.Marshalled;

namespace NJasmineTests.Specs
{
    public class Should_be_able_to_load_tests_through_AppDomain : GivenWhenThenFixture
    {
        public override void Specify()
        {
            var someDllPath = arrange(() => GetPathOfBinDeployed("SomeTestLibrary.dll"));

            expect(() => File.Exists(someDllPath));

            given("an AppDomain wrapper for a test DLL", delegate()
            {
                var appDomainWrapper = arrange(() => new AppDomainWrapper(someDllPath));

                then("objects can be instantiated in the AppDomain", () =>
                {
                    var o = appDomainWrapper.CreateObject<NJasmine.Marshalled.Executor>("NJasmine.dll");
                });

                then("the test assembly's configuration file is loaded", () =>
                {
                    var o = appDomainWrapper.CreateObject<NJasmine.Marshalled.Executor.AppSettingLoader>("NJasmine.dll");

                    string result = o.Get("someConfigurationValue");

                    expect(() => result == "#winning");
                });

                then("the tests can be enumerated", () =>
                {
                    var result = UsingAppDomain.LoadTestNames(appDomainWrapper, someDllPath);

                    expect(() => result.Contains("SomeTestLibrary.ASingleTest, first test"));
                });
            });
        }

        private string GetPathOfBinDeployed(string filename)
        {
            var currentDllDirectory = new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath).Directory;

            if (currentDllDirectory.Name == "bin")
                return Path.Combine(currentDllDirectory.FullName, "SomeTestLibrary.dll");
            else
                return Path.Combine(currentDllDirectory.FullName, "..\\..\\..\\SomeTestLibrary\\bin\\debug\\SomeTestLibrary.dll");
        }
    }
}
