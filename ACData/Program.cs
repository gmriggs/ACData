using System;
using System.Collections.Generic;
using System.IO;

using ACE.Adapter.GDLE.Models;
using ACE.Adapter.Lifestoned;

using ACE.Database.SQLFormatters;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ACData
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new UxMain());
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
                    Process(filename, new List<string>() { "*.json", "*.sql" }, (fi) => Convert(fi));
                    break;

                case "minify":
                    Process(filename, new List<string>() { "*.json" }, (fi) => Minify(fi));
                    break;

                case "indent":
                    Process(filename, new List<string>() { "*.json" }, (fi) => Indent(fi));
                    break;
            }
        }

        public static void Process(string path, string searchPattern, Action<FileInfo> callback)
        {
            Process(path, new List<string>() { searchPattern }, callback);
        }
        
        public static void Process(string path, List<string> searchPatterns, Action<FileInfo> callback)
        {
            if (File.Exists(path))
            {
                callback(new FileInfo(path));
                return;
            }

            if (Directory.Exists(path))
            {
                var di = new DirectoryInfo(path);

                var files = new List<FileInfo>();
               
                foreach (var pattern in searchPatterns)
                    files.AddRange(di.GetFiles(pattern));

                foreach (var file in files)
                    callback(file);

                var folders = di.GetDirectories();

                foreach (var folder in folders)
                    Process(folder.FullName, searchPatterns, callback);

                return;
            }

            // process wildcard patterns
            var searchPattern = path;
            var baseFolder = new DirectoryInfo(Directory.GetCurrentDirectory());

            var idx = path.LastIndexOf(Path.DirectorySeparatorChar);
            if (idx != -1)
            {
                var baseFolderStr = path.Substring(0, idx);
                if (!Directory.Exists(baseFolderStr))
                {
                    Console.WriteLine($"Couldn't find folder {baseFolderStr}");
                    return;
                }
                baseFolder = new DirectoryInfo(baseFolderStr);
                searchPattern = path.Substring(idx + 1);
            }

            var wcFiles = Directory.GetFiles(baseFolder.FullName, searchPattern);
            foreach (var file in wcFiles)
                Process(file, searchPattern, callback);
        }

        public static bool Convert(FileInfo fi)
        {
            if (fi.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                return json2sql(fi);
            else if (fi.Name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                return sql2json(fi);
            else
                return false;
        }

        public static bool sql2json(FileInfo fi)
        {
            var lines = File.ReadAllLines(fi.FullName);

            var sqlReader = new SQLReader();
            var weenie = sqlReader.sql2weenie(lines);

            if (!LifestonedConverter.TryConvertACEWeenieToLSDJSON(weenie, out var json, out var json_weenie))
            {
                Console.WriteLine($"Failed to convert {fi.FullName} to json");
                return false;
            }

            var metadata = Metadata.ReadData(string.Join("", lines));
            if (metadata != null && LifestonedLoader.AppendMetadata(json_weenie, metadata))
            {
                json = JsonConvert.SerializeObject(json_weenie, LifestonedConverter.SerializerSettings);
            }

            var jsonFilename = fi.FullName.Replace(".sql", ".json");
            File.WriteAllText(jsonFilename, json);

            Console.WriteLine($"Converted {fi.FullName} to {jsonFilename}");

            return true;
        }

        public static WeenieSQLWriter Converter;
        
        public static bool json2sql(FileInfo fi)
        {
            if (Converter == null)
            {
                Converter = new WeenieSQLWriter();
                Converter.WeenieNames = IDToString.Reader.GetIDToNames("WeenieName.txt");
                Converter.SpellNames = IDToString.Reader.GetIDToNames("SpellName.txt");
                Converter.TreasureDeath = IDToString.Reader.GetIDToTier("TreasureDeath.txt");
                Converter.TreasureWielded = IDToString.Reader.GetIDToWieldList("TreasureWielded.txt");
            }

            // read json into lsd weenie
            if (!LifestonedLoader.TryLoadWeenie(fi.FullName, out var weenie))
            {
                Console.WriteLine($"Failed to parse {fi.FullName}");
                return false;
            }

            // convert to ace weenie
            if (!LifestonedConverter.TryConvert(weenie, out var output))
            {
                Console.WriteLine($"Failed to convert {fi.FullName}");
                return false;
            }

            // output to sql
            try
            {
                if (output.LastModified == DateTime.MinValue)
                    output.LastModified = DateTime.UtcNow;

                var sqlFilename = Converter.GetDefaultFileName(output);
                var sqlFile = new StreamWriter(fi.DirectoryName + Path.DirectorySeparatorChar + sqlFilename);

                Converter.CreateSQLDELETEStatement(output, sqlFile);
                sqlFile.WriteLine();

                Converter.CreateSQLINSERTStatement(output, sqlFile);

                var metadata = new ACE.Adapter.GDLE.Models.Metadata(weenie);
                if (metadata.HasInfo)
                {
                    var jsonEx = JsonConvert.SerializeObject(metadata, Formatting.Indented);
                    sqlFile.WriteLine($"\n/* Lifestoned Changelog:\n{jsonEx}\n*/");
                }

                sqlFile.Close();

                Console.WriteLine($"Converted {fi.FullName} to {fi.DirectoryName}{Path.DirectorySeparatorChar}{sqlFilename}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine($"Failed to convert {fi.FullName}");
                return false;
            }

            return true;
        }

        public static void Minify(FileInfo fi)
        {
            FormatJson(fi, Formatting.None);
        }

        public static void Indent(FileInfo fi)
        {
            FormatJson(fi, Formatting.Indented);
        }

        public static void FormatJson(FileInfo fi, Formatting f)
        {
            var json = File.ReadAllText(fi.FullName);
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = f };
                jsonWriter.WriteToken(jsonReader);

                File.WriteAllText(fi.FullName, stringWriter.ToString());

                Console.WriteLine($"{(f == Formatting.Indented ? "Indented" : "Minified")} {fi.FullName}");
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
