using System;
using System.IO;
using ManyConsole;
using NJasmineTests.Export;
using NJasmineTests.Specs.expectations;

namespace NJasmineTestLoader
{
    class VerifyTestCommand : ConsoleCommand
    {
        public VerifyTestCommand()
        {
            Command = "verify-test";
            OneLineDescription = "Verifies the output of a test run meets specification.";
            TraceCommandAfterParse = false;
        }

        public override void FinishLoadingArguments(string[] remainingArguments)
        {
            VerifyNumberOfArguments(remainingArguments, 3);

            TestName = remainingArguments[0];
            XmlOutputFile = remainingArguments[1];
            ConsoleOutputFile = remainingArguments[2];
        }

        public string TestName;
        public string XmlOutputFile;
        public string ConsoleOutputFile;

        public override int Run()
        {
            var xmlOutput = File.ReadAllText(XmlOutputFile);
            var consoleOutput = File.ReadAllText(ConsoleOutputFile);

            var testType = typeof (can_check_that_an_arbtirary_condition_is_true).Assembly.GetType(TestName);
            var test = testType.GetConstructor(new Type[0]).Invoke(new Type[0]) as INJasmineInternalRequirement;

            var fixtureResult = new FixtureResult(TestName, xmlOutput, consoleOutput);

            test.Verify_NJasmine_implementation(fixtureResult);

            return 0;
        }
    }
}