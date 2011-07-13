using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NJasmineTests;

namespace NJasmineTestLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            RunExternalAttribute.TestDefinition[] testsToRun = RunExternalAttribute.GetAll().ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof (RunExternalAttribute.TestDefinition[]));
            serializer.Serialize(Console.Out, testsToRun);
        }
    }
}
