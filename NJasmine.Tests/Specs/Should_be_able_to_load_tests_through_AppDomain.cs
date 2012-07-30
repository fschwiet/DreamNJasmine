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
                    var result = WithinAppDomain.LoadTestNames(someDllPath, appDomainWrapper);

                    expect(() => result.Contains("SomeTestLibrary.ASingleTest, first test"));
                });

                then("the tests can be ran", () =>
                {
                    var listener = new Mock<ITestResultListener>();
                    WithinAppDomain.RunTests(someDllPath, appDomainWrapper, new string[]
                    {
                        "ASingleTest.first test"
                    }, listener.Object);

                    listener.Verify(l => l.NotifyStart(It.IsAny<TestContext>()));
                    listener.Verify(l => l.NotifyEnd(It.IsAny<TestContext>(), It.IsAny<TestResultShim>()));
                });
            });
        }

        private string GetPathOfBinDeployed(string filename)
        {
            var currentDllDirectory = new FileInfo(new Uri(this.GetType().Assembly.CodeBase).LocalPath).Directory;

            string otherDllDirectory = null;

            if (currentDllDirectory.Name == "build")
                otherDllDirectory = Path.Combine(currentDllDirectory.FullName, filename);
            else
                otherDllDirectory = Path.Combine(currentDllDirectory.FullName, "..\\..\\..\\SomeTestLibrary\\bin\\debug\\SomeTestLibrary.dll");

            return Path.GetFullPath(otherDllDirectory);
        }
    }
}
