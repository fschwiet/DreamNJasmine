using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NJasmineTests;

namespace NJasmineTestLoader
{
    class Program
    {
        static int Main(string[] args)
        {
            var commands = new AbstractCommand[]
            {
                new ListTestsCommand(),
                new VerifyTestCommand()
            };

            AbstractCommand command = null;

            try
            {
                if (args.Count() < 1)
                    throw new ArgumentException("No arguments specified.");

                foreach (var possibleCommand in commands)
                {
                    if (args.First().ToLower() == possibleCommand.Name)
                    {
                        command = possibleCommand;

                        command.LoadArgs(args.Skip(1).ToArray());
                        break;
                    }
                }

                if (command == null)
                    throw new ArgumentException("First parameter not recognized.");
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("Error: " + e.Message);

                if (command != null)
                    command.WriteExpectedArguments(Console.Out, "NJasmine.TestUtil");
                else
                {
                    Console.WriteLine("First parameter should be one of: " + String.Join(", ", commands.Select(c => c.Name).ToArray()));
                }

                return -1;
            }

            try
            {
                command.Trace(Console.Out);

                return command.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("Caught unhandled exception: " + e.ToString());
                return -1;
            }
        }
    }
}
