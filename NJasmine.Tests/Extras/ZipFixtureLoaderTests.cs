using System;
using System.IO;
using NJasmine;
using NJasmine.Extras;
using Should.Fluent;
using Should.Fluent.Model;

namespace NJasmineTests.Extras
{
    public class ZipFixtureLoaderTests : NJasmineFixture
    {
        public override void Tests()
        {
            it("gives a useful exception if the installation zip is not found", delegate
            {
                expect((Action)delegate
                {
                    var result =
                        ZipFixtureLoader.UnzipBinDeployedToTempDirectory(
                            "incorrectResourcePath.zip", "NJasmine.Extras");

                }).to.Throw<FileNotFoundException>();
            });

            it("can decompress installation zip", delegate
            {
                string unpacked = ZipFixtureLoader.UnzipBinDeployedToTempDirectory("Extras\\SampleZipFixture.zip", "NJasmine.Extras");

                Directory.Exists(unpacked).Should().Be.True();
                unpacked.ToLower().Should().Contain("temp");
                
                string expectedFixtureMember = Path.Combine(unpacked, "success.txt");
                
                File.Exists(expectedFixtureMember).Should().Be.True();
                File.ReadAllText(expectedFixtureMember).Should().Equal("true");
            });
        }
    }
}
