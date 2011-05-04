using System;
using System.IO;
using NJasmine;
using NJasmine.Extras;
using NUnit.Framework;

namespace NJasmineTests.Extras
{
    public class ZipDeployToolsTest : GivenWhenThenFixture
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

                expect(() => Directory.Exists(unpacked));
                expect(() => unpacked.ToLower().Contains("temp"));
                
                string expectedFixtureMember = Path.Combine(unpacked, "success.txt");

                expect(() => File.Exists(expectedFixtureMember));
                expect(() => File.ReadAllText(expectedFixtureMember) == "Hello, World\r\n");
            });

            it("decompressed zip includes empty folders", delegate
            {
                string unpacked = ZipDeployTools.UnzipBinDeployedToTempDirectory("Extras\\Sample.zip", "NJasmine.Extras");
                string expectedDirectory = Path.Combine(unpacked, "empty");

                expect(() => Directory.Exists(expectedDirectory));
            });
        }
    }
}
