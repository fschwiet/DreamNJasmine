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
            this.IsCommand("list-tests", "Lists the tests that are NJasmine specifications.");
            this.SkipsCommandSummaryBeforeRunning();
            this.HasAdditionalArguments(0);
        }

        public override int Run(string[] args)
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