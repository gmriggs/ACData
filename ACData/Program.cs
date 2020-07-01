using System;
using System.Collections.Generic;

using ACDataLib;

namespace ACData
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return;
            }

            var command = "";
            var filename = "";
            
            if (args.Length == 1)
            {
                // assume convert
                command = "convert";
                filename = args[0];
            }
            else
            {
                command = args[0].ToLower();

                if (!command.Equals("convert") && !command.Equals("minify") && !command.Equals("indent"))
                {
                    Console.WriteLine($"Unknown command: {command}");
                    Usage();
                    return;
                }
                filename = args[1];
            }

            switch (command)
            {
                case "convert":
                    Converter.Process(filename, new List<string>() { "*.json", "*.sql" }, (fi) => Converter.Convert(fi));
                    break;

                case "minify":
                    Converter.Process(filename, new List<string>() { "*.json" }, (fi) => Converter.Minify(fi));
                    break;

                case "indent":
                    Converter.Process(filename, new List<string>() { "*.json" }, (fi) => Converter.Indent(fi));
                    break;
            }
        }

        public static void Usage()
        {
            Console.WriteLine("ACData - A tool for converting Asheron's Call server data formats\n");
            Console.WriteLine("Usage:");
            Console.WriteLine("   acdata convert <file or folder>");
            Console.WriteLine("    - Converts .json files into .sql files");
            Console.WriteLine("    - Converts .sql files into .json files\n");
            Console.WriteLine("   acdata minify <file or folder>");
            Console.WriteLine("    - Converts .json files from indented to minified format\n");
            Console.WriteLine("   acdata indent <file or folder>");
            Console.WriteLine("    - Converts .json files from minified to indented format");
        }
    }
}
