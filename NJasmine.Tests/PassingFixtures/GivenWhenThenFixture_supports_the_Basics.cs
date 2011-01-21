using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;

namespace NJasmineTests.PassingFixtures
{
    [RunExternal(true, ExpectedTestNames = new[]
    {
        "NJasmineTests.PassingFixtures.GivenWhenThenFixture_supports_the_Basics, given the environment is in a particular state, when the system under test is used in a particular manner, then a particular result is expected", 
        "NJasmineTests.PassingFixtures.GivenWhenThenFixture_supports_the_Basics, given the environment is in a particular state, given the environment is in an even more particular state, when the system is used in another manner, then another result is expected",
        "NJasmineTests.PassingFixtures.GivenWhenThenFixture_supports_the_Basics, given the environment is in a particular state, given the environment is in an even more particular state, when the system is used in another manner, then yet another result is expected",
    })]
    public class GivenWhenThenFixture_supports_the_Basics : GivenWhenThenFixture
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
                        expect(() => File.ReadAllText(path) == "Hello, World");
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
    }
}
