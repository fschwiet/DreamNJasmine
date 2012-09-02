using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ManyConsole;
using NJasmineTests.Export;

namespace NJasmineTestLoader
{
    public class VerifyTestForVS2012Command : ConsoleCommand
    {
        public VerifyTestForVS2012Command()
        {
            this.IsCommand("verify-test-vs2012", "Verifies the output of a NUnit test run meets specification.");
            this.SkipsCommandSummaryBeforeRunning();

            this.HasAdditionalArguments(3, "<testName> <trvOutputFile> <consoleOutputFile>");
        }

        public string TestName;
        public string TrvOutputFile;
        public string ConsoleOutputFile;

        public override int Run(string[] remainingArgs)
        {
            TestName = remainingArgs[0];
            TrvOutputFile = remainingArgs[1];
            ConsoleOutputFile = remainingArgs[2];

            if (!File.Exists(TrvOutputFile))
                throw new ConsoleHelpAsException("Could not find trv output file at '" + TrvOutputFile + "'.");

            if (!File.Exists(ConsoleOutputFile))
                throw new ConsoleHelpAsException("Could not find console output file at '" + ConsoleOutputFile + "'.");

            var testContstructor = VerifyTestForNUnitCommand.GetTestConstructor(TestName);

            var trvContents = File.ReadAllText(TrvOutputFile);
            var consoleContents = File.ReadAllText(ConsoleOutputFile);

            var fixtureResult = new VS2012FixtureResult(trvContents);

            try
            {
                testContstructor.Verify_NJasmine_implementation(fixtureResult);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception thrown:");
                Console.WriteLine(e.ToString());
                return -1;
            }

            return 0;
        }
    }
}
