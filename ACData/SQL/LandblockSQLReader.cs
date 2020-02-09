using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

using ACE.Database.Models.World;

namespace ACData
{
    public class LandblockSQLReader: SQLReader
    {
        private ushort LandblockId;

        private List<LandblockInstance> LandblockInstances;

        private List<LandblockInstanceLink> LandblockInstanceLinks;
        
        public List<LandblockInstance> ReadModel(string[] sqlLines)
        {
            LandblockInstances = new List<LandblockInstance>();
            LandblockInstanceLinks = new List<LandblockInstanceLink>();

            foreach (var line in sqlLines)
            {
                var match = Regex.Match(line, @"`landblock` = 0x([\dA-F]+);");
                if (match.Success)
                {
                    ushort.TryParse(match.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out LandblockId);
                    //Console.WriteLine($"Found landblock id {LandblockId:X4}");
                    continue;
                }

                match = Regex.Match(line, @"INSERT INTO `([^`]+)");
                if (match.Success)
                {
                    CurrentTable = GetTable(match.Groups[1].Value);
                    //Console.WriteLine(CurrentTable);

                    CurrentColumns = GetColumns(line);
                    //Console.WriteLine($"Found columns: {string.Join(", ", CurrentColumns)}");
                    continue;
                }

                if (line.Contains("("))
                {
                    var fields = GetFields(line);
                    //Console.WriteLine($"Found fields: {string.Join(", ", fields)}");

                    AddRecord(fields);
                }
            }

            BuildLinks();

            return LandblockInstances;
        }

        public void BuildLinks()
        {
            foreach (var instance in LandblockInstances)
                instance.Landblock = LandblockId;

            foreach (var link in LandblockInstanceLinks)
            {
                var parent = LandblockInstances.FirstOrDefault(i => i.Guid == link.ParentGuid);
                if (parent == null)
                {
                    Console.WriteLine($"BuildLinks() - couldn't find parent guid {link.ParentGuid:X8}");
                    continue;
                }
                parent.LandblockInstanceLink.Add(link);
            }
        }

        public void AddRecord(List<string> fields)
        {
            if (CurrentTable == Table.LandblockInstance)
            {
                var record = new LandblockInstance();
                PopulateFields(record, fields);
                LandblockInstances.Add(record);
            }
            else if (CurrentTable == Table.LandblockInstanceLink)
            {
                var record = new LandblockInstanceLink();
                PopulateFields(record, fields);
                LandblockInstanceLinks.Add(record);
            }
        }

        public static Table GetTable(string table)
        {
            if (table.Equals("landblock_instance"))
                return Table.LandblockInstance;
            else if (table.Equals("landblock_instance_link"))
                return Table.LandblockInstanceLink;
            else
                return Table.None;
        }
    }
}
