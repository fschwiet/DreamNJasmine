using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using NJasmineTests;
using NJasmineTests.Export;
using NJasmineTests.Specs.expectations;

namespace NJasmineTestLoader
{
    public class ListTestsCommand : AbstractCommand
    {
        public override string Name
        {
            get { return "list-tests"; }
        }

        public override void LoadArgs(string[] args)
        {
            if (args.Length != 0)
                throw new ArgumentException("Unexpected arguments for command " + Name);
        }

        public override int Run()
        {
            var types = from t in typeof(can_check_that_an_arbtirary_condition_is_true).Assembly.GetTypes()
                        where t.GetInterfaces().Contains(typeof(INJasmineInternalRequirement))
                        where t.IsPublic
                   select new ListTestsCommand.TestDefinition
                   {
                       Name = t.FullName
                   };

            XmlSerializer serializer = new XmlSerializer(typeof(ListTestsCommand.TestDefinition[]));
            serializer.Serialize(Console.Out, types.ToArray());

            return 0;
        }

        public override void WriteExpectedArguments(TextWriter tw, string exeName)
        {
            tw.WriteLine(exeName + " list-tests");
        }

        public override void Trace(TextWriter tw)
        {
            // no output since script reads it directly
        }

        public class TestDefinition
        {
            public string Name;
        }
    }
}