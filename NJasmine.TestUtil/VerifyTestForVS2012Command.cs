using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ManyConsole;

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

            Console.WriteLine("WAT DO?!?");
            return 0;
        }
    }
}
