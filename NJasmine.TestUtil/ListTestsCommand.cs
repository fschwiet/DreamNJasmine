using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ManyConsole;
using NJasmineTests;
using NJasmineTests.Export;
using NJasmineTests.Specs.expectations;

namespace NJasmineTestLoader
{
    public class ListTestsCommand : ConsoleCommand
    {
        public ListTestsCommand()
        {
            Command = "list-tests";
            OneLineDescription = "Lists the tests that are NJasmine specifications.";
            TraceCommandAfterParse = false;
        }

        public override void FinishLoadingArguments(string[] remainingArguments)
        {
            VerifyNumberOfArguments(remainingArguments, 0);
        }

        public override int Run()
        {
            var types = from t in typeof(can_check_that_an_arbtirary_condition_is_true).Assembly.GetTypes()
                        where t.GetInterfaces().Contains(typeof(INJasmineInternalRequirement))
                        where t.IsPublic
                        orderby t.FullName
                   select new ListTestsCommand.TestDefinition
                   {
                       Name = t.FullName
                   };

            XmlSerializer serializer = new XmlSerializer(typeof(ListTestsCommand.TestDefinition[]));
            serializer.Serialize(Console.Out, types.ToArray());

            return 0;
        }

        public class TestDefinition
        {
            public string Name;
        }
    }
}