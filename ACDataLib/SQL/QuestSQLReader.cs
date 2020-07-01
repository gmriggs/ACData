using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ACE.Database.Models.World;

namespace ACDataLib
{
    public class QuestSQLReader : SQLReader
    {
        private string QuestName;

        private Quest Quest;

        public Quest ReadModel(string[] sqlLines)
        {
            Quest = new Quest();

            foreach (var line in sqlLines)
            {
                var match = Regex.Match(line, @"`name` = '([^']+)");
                if (match.Success)
                {
                    QuestName = match.Groups[1].Value;
                    //Console.WriteLine($"Found quest {QuestName}");
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

            return Quest;
        }

        private void AddRecord(List<string> fields)
        {
            if (CurrentTable == Table.Quest)
            {
                Quest = new Quest();
                PopulateFields(Quest, fields);
            }
        }

        private static Table GetTable(string table)
        {
            if (table.Equals("quest"))
                return Table.Quest;
            else
                return Table.None;
        }
    }
}
