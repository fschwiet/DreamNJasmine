using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ManyConsole;
using NJasmineTests;

namespace NJasmineTestLoader
{
    class Program
    {
        static int Main(string[] args)
        {
            var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(ListTestsCommand));

            return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }
    }
}
