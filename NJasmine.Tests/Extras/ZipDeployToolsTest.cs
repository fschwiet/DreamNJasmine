using System;
using System.IO;
using NJasmine;
using NJasmine.Extras;
using NUnit.Framework;
using Should.Fluent;
using Should.Fluent.Model;

namespace NJasmineTests.Extras
{
    public class ZipDeployToolsTest : NJasmineFixture
    {
        public override void Specify()
        {
            it("gives a useful exception if the installation zip is not found", delegate
            {
                Assert.Throws<FileNotFoundException>(delegate
                {
                    var result =
                        ZipDeployTools.UnzipBinDeployedToTempDirectory(
                            "incorrectResourcePath.zip", "NJasmine.Extras");

                });
            });

            it("can decompress installation zip", delegate
            {
                string unpacked = ZipDeployTools.UnzipBinDeployedToTempDirectory("Extras\\Sample.zip", "NJasmine.Extras");

                Directory.Exists(unpacked).Should().Be.True();
                unpacked.ToLower().Should().Contain("temp");
                
                string expectedFixtureMember = Path.Combine(unpacked, "success.txt");
                
                File.Exists(expectedFixtureMember).Should().Be.True();
                File.ReadAllText(expectedFixtureMember).Should().Equal("Hello, World\r\n");
            });

            it("decompressed zip includes empty folders", delegate
            {
                string unpacked = ZipDeployTools.UnzipBinDeployedToTempDirectory("Extras\\Sample.zip", "NJasmine.Extras");
                string expectedDirectory = Path.Combine(unpacked, "empty");

                Directory.Exists(expectedDirectory).Should().Be.True();
            });
        }
    }
}
