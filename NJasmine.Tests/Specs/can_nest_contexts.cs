using System;
using System.IO;
using NJasmine;
using NJasmine.Marshalled;
using NJasmineTests.Export;

namespace NJasmineTests.Specs
{
    public class can_nest_contexts : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            given("the environment is in a particular state", delegate
            {
                string path = Path.Combine(Path.GetTempPath(), "simple_GivenWhenThen.txt");

                cleanup(delegate
                {
                    if (File.Exists(path))
                        File.Delete(path);
                });

                when("the system under test is used in a particular manner", delegate
                {
                    var stream = arrange(() => File.Open(path, FileMode.CreateNew, FileAccess.Write));
                    var writer = arrange(() => new StreamWriter(stream));

                    arrange(() => writer.WriteLine("Hello, world."));

                    then("a particular result is expected", delegate
                    {
                        expect(() => File.Exists(path));
                    });
                });

                given("the environment is in an even more particular state", delegate
                {
                    string path2 = Path.Combine(Path.GetTempPath(), "simple_GivenWhenThen_copied.txt");

                    cleanup(delegate
                    {
                        if (File.Exists(path2))
                            File.Delete(path2);
                    });

                    when("the system is used in another manner", delegate
                    {
                        arrange(delegate
                        {
                            File.WriteAllText(path, "IMPORTANT MESSAGE");
                            File.Copy(path, path2);
                        });

                        then("another result is expected", delegate
                        {
                            expect(() => File.ReadAllText(path) == "IMPORTANT MESSAGE");
                        });

                        then("yet another result is expected", delegate
                        {
                            expect(() => File.ReadAllText(path2) == "IMPORTANT MESSAGE");
                        });
                    });
                });
            });
        }

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.succeeds();

            fixtureResult.hasTest("given the environment is in a particular state, when the system under test is used in a particular manner, then a particular result is expected");
            fixtureResult.hasTest("given the environment is in a particular state, given the environment is in an even more particular state, when the system is used in another manner, then another result is expected");
            fixtureResult.hasTest("given the environment is in a particular state, given the environment is in an even more particular state, when the system is used in another manner, then yet another result is expected");
        }
    }
}
