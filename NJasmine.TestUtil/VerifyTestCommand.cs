﻿using System;
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
            this.IsCommand("verify-test", "Verifies the output of a test run meets specification.");
            this.SkipsCommandSummaryBeforeRunning();

            this.HasAdditionalArguments(3, "<testName> <xmlOutputFile> <consoleOutputFile>");
        }

        public string TestName;
        public string XmlOutputFile;
        public string ConsoleOutputFile;

        public override int Run(string[] remainingArgs)
        {
            TestName = remainingArgs[0];
            XmlOutputFile = remainingArgs[1];
            ConsoleOutputFile = remainingArgs[2];

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