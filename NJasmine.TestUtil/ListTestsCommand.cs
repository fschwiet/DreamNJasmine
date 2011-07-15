using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using NJasmineTests;
using NJasmineTests.Export;

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
            var types = from t in typeof(NJasmineTests.Specs.expect).Assembly.GetTypes()
                        where t.GetInterfaces().Contains(typeof(INJasmineInternalRequirement))
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