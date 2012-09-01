using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class NUnitSuiteResultTest : GivenWhenThenFixture
    {
        public override void Specify()
        {
            describe("thatsInconclusive()", delegate
            {
                it("allows results that are inconclusive", delegate
                {
                    var xmlOutput = NUnitFixtureResult.GetSampleXmlResult(aSuiteName: "fooSuite", aSuiteResult: "Inconclusive");

                    var sut = new NUnitFixtureResult("ignored", xmlOutput).hasSuite("fooSuite");

                    sut.thatsInconclusive();
                });

                it("gives an error indicating the actual result when not inconclusive", delegate
                {
                    var xmlOutput = NUnitFixtureResult.GetSampleXmlResult(aSuiteName: "fooSuite", aSuiteResult: "OtherResult");

                    var sut = new NUnitFixtureResult("ignored", xmlOutput).hasSuite("fooSuite");

                    var exception = Assert.Throws(NUnitFixtureResultTest.ExpectedAssertionType, delegate
                    {
                        sut.thatsInconclusive();
                    });

                    expect(() => exception.Message.Contains("fooSuite"));
                    expect(() => exception.Message.Contains("OtherResult"));
                });
            });

            describe("thatHasNoResults()", delegate
            {
                it("fails if the suite has results", delegate
                {
                    var sut = new NUnitFixtureResult("ignored",
                        NUnitFixtureResult.GetSampleXmlResult(aSuiteName: "someSuiteHello"))
                        .hasSuite("someSuiteHello");

                    var exception = Assert.Throws(NUnitFixtureResultTest.ExpectedAssertionType, delegate
                    {
                        sut.thatHasNoResults();
                    });

                    expect(() => exception.Message.Contains("someSuiteHello"));
                });
            });
        }
    }
}
