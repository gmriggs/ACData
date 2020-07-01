using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using ACE.Database.Models.World;

namespace ACDataLib.IDToString
{
    public static class Reader
    {
        public static Dictionary<uint, string> GetIDToNames(string filename)
        {
            var records = ReadCSV(filename);

            var results = new Dictionary<uint, string>();

            for (var i = 0; i < records.Count; i++)
            {
                var record = records[i];

                if (record.Length < 2)
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: expected 2 fields in {string.Join(",", record)}");
                    continue;
                }

                if (record.Length > 2)
                    record[1] = string.Join(",", record.Skip(1));

                if (!uint.TryParse(record[0], out var id))
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: {record[0]} is not an unsigned int");
                    continue;
                }

                if (results.ContainsKey(id))
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: {record[0]} is a duplicate key");
                    continue;
                }

                results.Add(id, record[1]);
            }

            return results;
        }

        public static Dictionary<uint, int> GetIDToTier(string filename)
        {
            var records = ReadCSV(filename);

            var results = new Dictionary<uint, int>();

            for (var i = 0; i < records.Count; i++)
            {
                var record = records[i];

                if (record.Length != 2)
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: expected 2 fields in {string.Join(",", record)}");
                    continue;
                }

                if (!uint.TryParse(record[0], out var id))
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: {record[0]} is not an unsigned int");
                    continue;
                }

                if (results.ContainsKey(id))
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: {record[0]} is a duplicate key");
                    continue;
                }

                if (!int.TryParse(record[1], out var tier))
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: {record[1]} is not a number");
                    continue;
                }

                results.Add(id, tier);
            }

            return results;
        }

        public static Dictionary<uint, List<TreasureWielded>> GetIDToWieldList(string filename)
        {
            var records = ReadCSV(filename);

            var results = new Dictionary<uint, List<TreasureWielded>>();

            for (var i = 0; i < records.Count; i++)
            {
                var record = records[i];

                // uint TreasureType
                // uint WeenieClassId
                // uint PaletteId
                // float Shade
                // int StackSize
                // float Probability

                if (record.Length != 6)
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: expected 6 fields in {string.Join(",", record)}");
                    continue;
                }

                if (!uint.TryParse(record[0], out var id))
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: {record[0]} is not an unsigned int");
                    continue;
                }

                if (!uint.TryParse(record[1], out var wcid))
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: {record[1]} is not an unsigned int");
                    continue;
                }

                if (!uint.TryParse(record[2], out var palette))
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: {record[2]} is not an unsigned int");
                    continue;
                }

                if (!float.TryParse(record[3], out var shade))
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: {record[3]} is not a decimal");
                    continue;
                }

                if (!int.TryParse(record[4], out var stacksize))
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: {record[4]} is not a number");
                    continue;
                }

                if (!float.TryParse(record[5], out var probability))
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in {filename}: {record[5]} is not a decimal");
                    continue;
                }

                if (!results.TryGetValue(id, out var list))
                {
                    list = new List<TreasureWielded>();
                    results.Add(id, list);
                }

                var treasure = new TreasureWielded();
                treasure.TreasureType = id;
                treasure.WeenieClassId = wcid;
                treasure.PaletteId = palette;
                treasure.Shade = shade;
                treasure.StackSize = stacksize;
                treasure.Probability = probability;

                list.Add(treasure);
            }

            return results;
        }

        public static List<string[]> ReadCSV(string filename)
        {
            var records = new List<string[]>();

            var lines = File.ReadAllLines("IDToString" + Path.DirectorySeparatorChar + filename);

            foreach (var line in lines)
            {
                if (line.StartsWith("//") || !line.Contains(","))
                    continue;

                records.Add(line.Split(','));
            }

            return records;
        }
    }
}
