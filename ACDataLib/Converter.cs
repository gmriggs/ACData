using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ACE.Adapter.GDLE;
using ACE.Adapter.GDLE.Models;
using ACE.Adapter.Lifestoned;

using ACE.Database.SQLFormatters;

using Newtonsoft.Json;

namespace ACDataLib
{
    public enum Mode
    {
        File,
        String
    };
    
    public enum FileFormat
    {
        SQL,
        JSON
    };

    public static class Converter
    {
        public static Mode Mode = Mode.File;
        
        // if Mode == String, write to Output string
        public static string Output;

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

        public static bool Convert(FileInfo fi, DirectoryInfo outputFolder = null)
        {
            if (fi.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                return json2sql(fi, null, outputFolder);
            else if (fi.Name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                return sql2json(fi, null, outputFolder);
            else
                return false;
        }

        public static bool Convert(string[] lines, FileFormat inputFormat)
        {
            var dummy = new FileInfo("");

            if (inputFormat == FileFormat.SQL)
                return sql2json(dummy, lines);
            else if (inputFormat == FileFormat.JSON)
                return json2sql(dummy, lines);
            else
                return false;
        }

        public static bool sql2json(FileInfo fi, string[] lines, DirectoryInfo outputFolder = null)
        {
            var contentType = SQLReader.GetContentType(fi.FullName);

            if (contentType == ContentType.Undefined)
            {
                Console.WriteLine($"Couldn't determine content type for {fi.FullName}");
                return false;
            }

            switch (contentType)
            {
                case ContentType.Landblock:
                    return sql2json_landblock(fi, lines, outputFolder);
                case ContentType.Quest:
                    return sql2json_quest(fi, lines, outputFolder);
                case ContentType.Recipe:
                    return sql2json_recipe(fi, lines, outputFolder);
                case ContentType.Weenie:
                    return sql2json_weenie(fi, lines, outputFolder);
                default:
                    return false;
            }
        }

        public static bool sql2json_weenie(FileInfo fi, string[] lines = null, DirectoryInfo outputFolder = null)
        {
            lines = lines ?? File.ReadAllLines(fi.FullName);

            var sqlReader = new WeenieSQLReader();
            var weenie = sqlReader.ReadModel(lines);

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

            if (Mode == Mode.String)
            {
                Output = json;
                return true;
            }

            var jsonFolder = outputFolder ?? fi.Directory;

            var jsonFilename = jsonFolder.FullName + Path.DirectorySeparatorChar + fi.Name.Replace(".sql", ".json");

            File.WriteAllText(jsonFilename, json);

            Console.WriteLine($"Converted {fi.FullName} to {jsonFilename}");

            return true;
        }

        public static bool sql2json_landblock(FileInfo fi, string[] lines = null, DirectoryInfo outputFolder = null)
        {
            lines = lines ?? File.ReadAllLines(fi.FullName);

            var sqlReader = new LandblockSQLReader();

            var landblockInstances = sqlReader.ReadModel(lines);

            if (GDLEConverter.WeenieNames == null)
                GDLEConverter.WeenieNames = IDToString.Reader.GetIDToNames("WeenieName.txt");

            if (GDLEConverter.WeenieClassNames == null)
                GDLEConverter.WeenieClassNames = IDToString.Reader.GetIDToNames("WeenieClassName.txt");

            if (!GDLEConverter.TryConvert(landblockInstances, out var result))
            {
                Console.WriteLine($"Failed to convert {fi.FullName} to json");
                return false;
            }

            var jsonFolder = outputFolder ?? fi.Directory;

            var jsonFilename = jsonFolder.FullName + Path.DirectorySeparatorChar + fi.Name.Replace(".sql", ".json");

            var json = JsonConvert.SerializeObject(result, LifestonedConverter.SerializerSettings);

            if (Mode == Mode.String)
            {
                Output = json;
                return true;
            }

            File.WriteAllText(jsonFilename, json);

            Console.WriteLine($"Converted {fi.FullName} to {jsonFilename}");

            return true;
        }

        public static bool sql2json_recipe(FileInfo fi, string[] lines = null, DirectoryInfo outputFolder = null)
        {
            lines = lines ?? File.ReadAllLines(fi.FullName);

            var sqlReader = new RecipeSQLReader();

            var cookbooks = sqlReader.ReadModel(lines);

            if (!GDLEConverter.TryConvert(cookbooks, out var result))
            {
                Console.WriteLine($"Failed to convert {fi.FullName} to json");
                return false;
            }

            var jsonFolder = outputFolder ?? fi.Directory;

            var jsonFilename = jsonFolder.FullName + Path.DirectorySeparatorChar + fi.Name.Replace(".sql", ".json");

            var json = JsonConvert.SerializeObject(result, LifestonedConverter.SerializerSettings);

            if (Mode == Mode.String)
            {
                Output = json;
                return true;
            }

            File.WriteAllText(jsonFilename, json);

            Console.WriteLine($"Converted {fi.FullName} to {jsonFilename}");

            return true;
        }

        public static bool sql2json_quest(FileInfo fi, string[] lines = null, DirectoryInfo outputFolder = null)
        {
            lines = lines ?? File.ReadAllLines(fi.FullName);

            var sqlReader = new QuestSQLReader();

            var quest = sqlReader.ReadModel(lines);

            if (!GDLEConverter.TryConvert(quest, out var result))
            {
                Console.WriteLine($"Failed to convert {fi.FullName} to json");
                return false;
            }

            var jsonFolder = outputFolder ?? fi.Directory;

            var jsonFilename = jsonFolder.FullName + Path.DirectorySeparatorChar + fi.Name.Replace(".sql", ".json");

            var json = JsonConvert.SerializeObject(result, LifestonedConverter.SerializerSettings);

            if (Mode == Mode.String)
            {
                Output = json;
                return true;
            }

            File.WriteAllText(jsonFilename, json);

            Console.WriteLine($"Converted {fi.FullName} to {jsonFilename}");

            return true;
        }

        public static bool json2sql(FileInfo fi, string[] lines = null, DirectoryInfo outputFolder = null)
        {
            var contentType = JsonReader.GetContentType(fi.FullName);

            if (contentType == ContentType.Undefined)
            {
                Console.WriteLine($"Couldn't determine content type for {fi.FullName}");
                return false;
            }

            switch (contentType)
            {
                case ContentType.Landblock:
                    return json2sql_landblock(fi, lines, outputFolder);
                case ContentType.Quest:
                    return json2sql_quest(fi, lines, outputFolder);
                case ContentType.Recipe:
                    return json2sql_recipe(fi, lines, outputFolder);
                case ContentType.Weenie:
                    return json2sql_weenie(fi, lines, outputFolder);
                default:
                    return false;
            }
        }

        public static WeenieSQLWriter WeenieSQLWriter;

        public static bool json2sql_weenie(FileInfo fi, string[] lines = null, DirectoryInfo outputFolder = null)
        {
            if (WeenieSQLWriter == null)
            {
                WeenieSQLWriter = new WeenieSQLWriter();
                WeenieSQLWriter.WeenieNames = IDToString.Reader.GetIDToNames("WeenieName.txt");
                WeenieSQLWriter.SpellNames = IDToString.Reader.GetIDToNames("SpellName.txt");
                WeenieSQLWriter.TreasureDeath = IDToString.Reader.GetIDToTier("TreasureDeath.txt");
                WeenieSQLWriter.TreasureWielded = IDToString.Reader.GetIDToWieldList("TreasureWielded.txt");
            }

            // read json into lsd weenie
            Lifestoned.DataModel.Gdle.Weenie weenie = null;
            if (lines == null)
            {
                if (!LifestonedLoader.TryLoadWeenie(fi.FullName, out weenie))
                {
                    Console.WriteLine($"Failed to parse {fi.FullName}");
                    return false;
                }
            }
            else
            {
                if (!LifestonedLoader.TryLoadWeenie(lines, out weenie))
                {
                    Console.WriteLine($"Failed to parse weenie");
                    return false;
                }
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

                StreamWriter sqlFile = null;
                string sqlFilename = null;
                MemoryStream memoryStream = null;

                if (lines == null)
                {
                    sqlFilename = WeenieSQLWriter.GetDefaultFileName(output);

                    var sqlFolder = outputFolder ?? fi.Directory;

                    sqlFile = new StreamWriter(sqlFolder.FullName + Path.DirectorySeparatorChar + sqlFilename);
                }
                else
                {
                    memoryStream = new MemoryStream();
                    sqlFile = new StreamWriter(memoryStream);
                }

                WeenieSQLWriter.CreateSQLDELETEStatement(output, sqlFile);
                sqlFile.WriteLine();

                WeenieSQLWriter.CreateSQLINSERTStatement(output, sqlFile);

                var metadata = new Metadata(weenie);
                if (metadata.HasInfo)
                {
                    var jsonEx = JsonConvert.SerializeObject(metadata, Formatting.Indented);
                    sqlFile.WriteLine($"\n/* Lifestoned Changelog:\n{jsonEx}\n*/");
                }

                sqlFile.Close();

                if (lines != null)
                {
                    Output = memoryStream.ToString();
                    return true;
                }

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

        public static LandblockSQLWriter LandblockSQLWriter;

        public static bool json2sql_landblock(FileInfo fi, string[] lines = null, DirectoryInfo outputFolder = null)
        {
            // read json into gdle spawnmap
            Landblock result = null;
            if (lines == null)
            {
                if (!GDLELoader.TryLoadLandblock(fi.FullName, out result))
                {
                    Console.WriteLine($"Failed to parse {fi.FullName}");
                    return false;
                }
            }
            else
            {
                if (!GDLELoader.TryLoadLandblock(lines, out result))
                {
                    Console.WriteLine($"Failed to parse landblock");
                    return false;
                }
            }

            // convert to sql landblock instances
            if (!GDLEConverter.TryConvert(result, out var landblockInstances, out var landblockInstanceLinks))
            {
                Console.WriteLine($"Failed to convert {fi.FullName}");
                return false;
            }

            // link up instances
            // TODO: move this to TryConvert
            foreach (var link in landblockInstanceLinks)
            {
                var parent = landblockInstances.FirstOrDefault(i => i.Guid == link.ParentGuid);
                if (parent == null)
                {
                    Console.WriteLine($"Couldn't find parent guid for {link.ParentGuid:X8}");
                    continue;
                }
                parent.LandblockInstanceLink.Add(link);

                var child = landblockInstances.FirstOrDefault(i => i.Guid == link.ChildGuid);
                if (child == null)
                {
                    Console.WriteLine($"Couldn't find child guid for {link.ChildGuid:X8}");
                    continue;
                }
                child.IsLinkChild = true;
            }

            // output to sql
            try
            {
                if (LandblockSQLWriter == null)
                {
                    LandblockSQLWriter = new LandblockSQLWriter();
                    LandblockSQLWriter.WeenieNames = IDToString.Reader.GetIDToNames("WeenieName.txt");
                }

                foreach (var landblockInstance in landblockInstances)
                {
                    if (landblockInstance.LastModified == DateTime.MinValue)
                        landblockInstance.LastModified = DateTime.UtcNow;
                }

                foreach (var landblockInstanceLink in landblockInstanceLinks)
                {
                    if (landblockInstanceLink.LastModified == DateTime.MinValue)
                        landblockInstanceLink.LastModified = DateTime.UtcNow;
                }

                StreamWriter sqlFile = null;
                string sqlFilename = null;
                MemoryStream memoryStream = null;
                
                if (lines == null)
                {
                    sqlFilename = LandblockSQLWriter.GetDefaultFileName(landblockInstances[0]);

                    var sqlFolder = outputFolder ?? fi.Directory;

                    sqlFile = new StreamWriter(sqlFolder.FullName + Path.DirectorySeparatorChar + sqlFilename);
                }
                else
                {
                    memoryStream = new MemoryStream();
                    sqlFile = new StreamWriter(memoryStream);
                }

                LandblockSQLWriter.CreateSQLDELETEStatement(landblockInstances, sqlFile);
                sqlFile.WriteLine();

                LandblockSQLWriter.CreateSQLINSERTStatement(landblockInstances, sqlFile);

                sqlFile.Close();

                if (lines != null)
                {
                    Output = memoryStream.ToString();
                    return true;
                }

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

        public static CookBookSQLWriter CookBookSQLWriter;
        public static RecipeSQLWriter RecipeSQLWriter;

        public static bool json2sql_recipe(FileInfo fi, string[] lines = null, DirectoryInfo outputFolder = null)
        {
            // read json into lsd recipe
            RecipeCombined result = null;
            if (lines == null)
            {
                if (!GDLELoader.TryLoadRecipeCombined(fi.FullName, out result))
                {
                    Console.WriteLine($"Failed to parse {fi.FullName}");
                    return false;
                }
            }
            else
            {
                if (!GDLELoader.TryLoadRecipeCombined(lines, out result))
                {
                    Console.WriteLine($"Failed to parse recipe");
                    return false;
                }
            }

            // convert to ace cookbooks + recipe
            if (!GDLEConverter.TryConvert(result, out var cookbooks, out var recipe))
            {
                Console.WriteLine($"Failed to convert {fi.FullName}");
                return false;
            }

            // output to sql
            try
            {
                if (RecipeSQLWriter == null)
                {
                    RecipeSQLWriter = new RecipeSQLWriter();
                    RecipeSQLWriter.WeenieNames = IDToString.Reader.GetIDToNames("WeenieName.txt");
                }

                if (CookBookSQLWriter == null)
                {
                    CookBookSQLWriter = new CookBookSQLWriter();
                    CookBookSQLWriter.WeenieNames = IDToString.Reader.GetIDToNames("WeenieName.txt");
                }

                if (recipe.LastModified == DateTime.MinValue)
                    recipe.LastModified = DateTime.UtcNow;

                foreach (var cookbook in cookbooks)
                {
                    if (cookbook.LastModified == DateTime.MinValue)
                        cookbook.LastModified = DateTime.UtcNow;
                }

                StreamWriter sqlFile = null;
                string sqlFilename = null;
                MemoryStream memoryStream = null;
                
                if (lines == null)
                {
                    sqlFilename = RecipeSQLWriter.GetDefaultFileName(recipe, cookbooks);

                    var sqlFolder = outputFolder ?? fi.Directory;

                    sqlFile = new StreamWriter(sqlFolder.FullName + Path.DirectorySeparatorChar + sqlFilename);
                }
                else
                {
                    memoryStream = new MemoryStream();
                    sqlFile = new StreamWriter(memoryStream);
                }

                RecipeSQLWriter.CreateSQLDELETEStatement(recipe, sqlFile);
                sqlFile.WriteLine();

                RecipeSQLWriter.CreateSQLINSERTStatement(recipe, sqlFile);
                sqlFile.WriteLine();

                CookBookSQLWriter.CreateSQLDELETEStatement(cookbooks, sqlFile);
                sqlFile.WriteLine();

                CookBookSQLWriter.CreateSQLINSERTStatement(cookbooks, sqlFile);

                sqlFile.Close();

                if (lines != null)
                {
                    Output = memoryStream.ToString();
                    return true;
                }

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

        public static QuestSQLWriter QuestSQLWriter;

        public static bool json2sql_quest(FileInfo fi, string[] lines = null, DirectoryInfo outputFolder = null)
        {
            // read json quest
            Quest result = null;
            if (lines == null)
            {
                if (!GDLELoader.TryLoadQuest(fi.FullName, out result))
                {
                    Console.WriteLine($"Failed to parse {fi.FullName}");
                    return false;
                }
            }
            else
            {
                if (!GDLELoader.TryLoadQuest(lines, out result))
                {
                    Console.WriteLine($"Failed to parse quest");
                    return false;
                }
            }

            // convert to sql quest
            if (!GDLEConverter.TryConvert(result, out var quest))
            {
                Console.WriteLine($"Failed to convert {fi.FullName}");
                return false;
            }

            // output to sql
            try
            {
                if (QuestSQLWriter == null)
                    QuestSQLWriter = new QuestSQLWriter();

                if (quest.LastModified == DateTime.MinValue)
                    quest.LastModified = DateTime.UtcNow;

                StreamWriter sqlFile = null;
                string sqlFilename = null;
                MemoryStream memoryStream = null;
                
                if (lines == null)
                {
                    sqlFilename = fi.Name.Replace(".json", ".sql");

                    var sqlFolder = outputFolder ?? fi.Directory;

                    sqlFile = new StreamWriter(sqlFolder.FullName + Path.DirectorySeparatorChar + sqlFilename);
                }
                else
                {
                    memoryStream = new MemoryStream();
                    sqlFile = new StreamWriter(memoryStream);
                }

                QuestSQLWriter.CreateSQLDELETEStatement(quest, sqlFile);
                sqlFile.WriteLine();

                QuestSQLWriter.CreateSQLINSERTStatement(quest, sqlFile);

                sqlFile.Close();

                if (lines != null)
                {
                    Output = memoryStream.ToString();
                    return true;
                }

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
    }
}
