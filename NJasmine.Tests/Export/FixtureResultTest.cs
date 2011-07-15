using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    [Explicit]
    public class FixtureResultTest : GivenWhenThenFixture
    {
        private const string _expectedFixtureName = "hello";
        Type expectedAssertionType = typeof(AssertionException);

        public override void Specify()
        {
            describe("succeeds()", delegate
            {
                it("allows a passing test result", delegate
                {
                    new FixtureResult(_expectedFixtureName, FixtureResult.GetSampleXmlResult(1)).succeeds();
                });

                var cases = new Dictionary<string, TestDelegate>();
                
                cases.Add("running against error", delegate
                {
                    var sut = new FixtureResult(_expectedFixtureName, FixtureResult.GetSampleXmlResult(1, 1));
                    sut.succeeds();
                });

                cases.Add("running against failure", delegate
                {
                    var sut = new FixtureResult(_expectedFixtureName, FixtureResult.GetSampleXmlResult(1, 0, 1));
                    sut.succeeds();
                });

                cases.Add("running against no tests", delegate
                {
                    var sut = new FixtureResult(_expectedFixtureName, FixtureResult.GetSampleXmlResult(0));
                    sut.succeeds();
                });

                CheckScenariosCauseErrorWithMessageContaining(cases, _expectedFixtureName);
            });

            describe("failed()", delegate
            {
                it("allows test results with errors or failures", delegate
                {
                    new FixtureResult(_expectedFixtureName, FixtureResult.GetSampleXmlResult(1, 1)).failed();
                    new FixtureResult(_expectedFixtureName, FixtureResult.GetSampleXmlResult(1, 0, 1)).failed();
                });

                CheckScenario("running against no tests", delegate
                {
                    var sut = new FixtureResult(_expectedFixtureName, FixtureResult.GetSampleXmlResult(0), "");
                    sut.failed();
                }, _expectedFixtureName);
            });

            describe("containsTrace", delegate
            {
                var originalXml = "<xml></xml>";
                    
                string originalConsole = @"
NUnit version 2.5.9.10348
Copyright (C) 2002-2009 Charlie Poole.
Copyright (C) 2002-2004 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov.
Copyright (C) 2000-2002 Philip Craig.
All Rights Reserved.

Runtime Environment -
   OS Version: Microsoft Windows NT 6.1.7601 Service Pack 1
  CLR Version: 2.0.50727.5446 ( Net 2.0 )

ProcessModel: Default    DomainUsage: Single
Execution Runtime: Default
<<{{test started, before include of a}}>>
<<{{after include of a}}>>
<<{{first describe, before include of b}}>>
<<{{after include of b}}>>
<<{{before include of c}}>>
<<{{after include of c}}>>
Selected test(s): NJasmineTests.Specs.beforeAll.beforeAll_and_afterAll_are_applied_to_the_correct_scope
.{{<<RESET>>}}
<<{{BEFORE ALL}}>>
<<{{first test}}>>
.<<{{SECOND BEFORE ALL}}>>
<<{{INNER BEFORE ALL}}>>
<<{{second test}}>>
.<<{{third test}}>>
<<{{INNER AFTER ALL}}>>
<<{{DISPOSING INNER BEFORE ALL}}>>
<<{{SECOND AFTER ALL}}>>
<<{{DISPOSING SECOND BEFORE ALL}}>>
<<{{AFTER ALL}}>>
<<{{DISPOSING BEFORE ALL}}>>

Tests run: 3, Errors: 0, Failures: 0, Inconclusive: 0, Time: 0.0820047 seconds
";
                
                it("allows tests with the expected trace", delegate
                {
                    new FixtureResult(_expectedFixtureName, originalXml, originalConsole).containsTrace(@"
BEFORE ALL
first test
SECOND BEFORE ALL
INNER BEFORE ALL
second test
third test
INNER AFTER ALL
DISPOSING INNER BEFORE ALL
SECOND AFTER ALL
DISPOSING SECOND BEFORE ALL
AFTER ALL
DISPOSING BEFORE ALL
");
                });

                it("fails tests without the expected trace", delegate
                {
                    var sut = new FixtureResult(_expectedFixtureName, originalXml, originalConsole);

                    var exception = Assert.Throws(expectedAssertionType, delegate
                    {
                        sut.containsTrace(@"
ONE
TWO
THREE
");
                    });

                    var message = exception.Message;

                    expect(() => message.IndexOf("ONE") < message.IndexOf("TWO"));
                    expect(() => message.IndexOf("TWO") < message.IndexOf("THREE"));

                    expect(() => message.IndexOf("BEFORE ALL") < message.IndexOf("INNER AFTER ALL"));
                    expect(() => message.IndexOf("INNER AFTER ALL") < message.IndexOf("DISPOSING BEFORE ALL"));

                    expect(() => message.Contains(_expectedFixtureName));
                });
            });

            describe("hasTest", delegate
            {
                var expectedTestName = "one_two_test";

                var xmlOutput = FixtureResult.GetSampleXmlResult(aTestName: expectedTestName);

                var sut = arrange(() => new FixtureResult(_expectedFixtureName, xmlOutput));

                it("returns a test by name", delegate
                {
                    expect(() => sut.hasTest(expectedTestName) != null);
                });

                it("gives a useful error message if the test is not found", delegate
                {
                    string wrongTestName = "fsadf325m";

                    var exception = Assert.Throws(expectedAssertionType, delegate
                    {
                        sut.hasTest(wrongTestName);
                    });

                    expect(() => exception.Message.Contains("Expected test not found, expected test named " + wrongTestName));
                });
            });

            describe("hasSuite", delegate
            {
                var expectedSuiteName = "one_two_test";

                var xmlOutput = FixtureResult.GetSampleXmlResult(aSuiteName: expectedSuiteName);

                var sut = arrange(() => new FixtureResult(_expectedFixtureName, xmlOutput));

                it("returns a test suite by name", delegate
                {
                    expect(() => sut.hasSuite(expectedSuiteName) != null);
                });

                it("gives a useful error message if the test suite is not found", delegate
                {
                    string wrongSuiteName = "9vasjf9d";

                    var exception = Assert.Throws(expectedAssertionType, delegate
                    {
                        sut.hasSuite(wrongSuiteName);
                    });

                    expect(() => exception.Message.Contains("Expected test suite not found, expected suite named " + wrongSuiteName));
                });
            });
        }

        private void CheckScenario(string scenarioName, TestDelegate scenarioAction, string expectedMessage)
        {
            CheckScenariosCauseErrorWithMessageContaining(new [] {new KeyValuePair<string, TestDelegate>(scenarioName, scenarioAction)}, expectedMessage);
        }

        private void CheckScenariosCauseErrorWithMessageContaining(IEnumerable<KeyValuePair<string, TestDelegate>> cases, string expected)
        {
            foreach (var scenario in cases)
            {
                it("asserts when " + scenario.Key, delegate
                {
                    var exception = Assert.Throws(expectedAssertionType, scenario.Value);

                    Assert.That(exception.Message, Is.StringContaining(expected));
                });
            }
        }
    }
}
