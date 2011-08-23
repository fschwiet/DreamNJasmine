using System;
using System.IO;
using NJasmineTests.Export;
using NJasmineTests.Specs.expectations;

namespace NJasmineTestLoader
{
    class VerifyTestCommand : AbstractCommand
    {
        public override string Name
        {
            get { return "verify-test"; }
        }

        public string TestName;
        public string XmlOutputFile;
        public string ConsoleOutputFile;

        public override void LoadArgs(string[] args)
        {
            if (args.Length != 3)
                throw new ArgumentException("Expected 3 additional arguments for verify-test");

            TestName = args[0];
            XmlOutputFile = args[1];
            ConsoleOutputFile = args[2];
        }

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

        public override void WriteExpectedArguments(TextWriter tw, string exeName)
        {
            tw.WriteLine(exeName + " " + Name + " <testName> <xmlOutputFile> <consoleOutputFile>");
        }

        public override void Trace(TextWriter tw)
        {
        }
    }
}