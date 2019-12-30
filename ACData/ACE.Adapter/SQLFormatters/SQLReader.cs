using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

using ACE.Database.Models.World;

namespace ACData
{
    public class SQLReader
    {
        public uint Wcid;

        public Table CurrentTable;

        public List<string> CurrentColumns;

        public Weenie Weenie;

        public WeeniePropertiesEmote CurrentEmote;

        public Weenie sql2weenie(string[] sqlLines)
        {
            Weenie = new Weenie();

            foreach (var line in sqlLines)
            {
                var match = Regex.Match(line, @"`class_Id` = (\d+);");
                if (match.Success)
                {
                    uint.TryParse(match.Groups[1].Value, out Wcid);
                    //Console.WriteLine($"Found wcid {Wcid}");
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

                if (line.Contains("SET @parent_id = LAST_INSERT_ID()"))
                    continue;

                if (line.Contains("("))
                {
                    var fields = GetFields(line);
                    //Console.WriteLine($"Found fields: {string.Join(", ", fields)}");

                    AddRecord(fields);
                }
            }
            return Weenie;
        }

        public void AddRecord(List<string> fields)
        {
            if (CurrentTable == Table.Weenie)
            {
                PopulateFields(Weenie, fields);
            }
            else if (CurrentTable == Table.AnimPart)
            {
                var record = new WeeniePropertiesAnimPart();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesAnimPart.Add(record);
            }
            else if (CurrentTable == Table.Attribute)
            {
                var record = new WeeniePropertiesAttribute();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesAttribute.Add(record);
            }
            else if (CurrentTable == Table.Attribute2nd)
            {
                var record = new WeeniePropertiesAttribute2nd();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesAttribute2nd.Add(record);
            }
            else if (CurrentTable == Table.BodyPart)
            {
                var record = new WeeniePropertiesBodyPart();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesBodyPart.Add(record);
            }
            else if (CurrentTable == Table.Book)
            {
                Weenie.WeeniePropertiesBook = new WeeniePropertiesBook();
                PopulateFields(Weenie.WeeniePropertiesBook, fields);
            }
            else if (CurrentTable == Table.BookPageData)
            {
                var record = new WeeniePropertiesBookPageData();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesBookPageData.Add(record);
            }
            else if (CurrentTable == Table.Bool)
            {
                var record = new WeeniePropertiesBool();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesBool.Add(record);
            }
            else if (CurrentTable == Table.CreateList)
            {
                var record = new WeeniePropertiesCreateList();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesCreateList.Add(record);
            }
            else if (CurrentTable == Table.DID)
            {
                var record = new WeeniePropertiesDID();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesDID.Add(record);
            }
            else if (CurrentTable == Table.Emote)
            {
                var record = new WeeniePropertiesEmote();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesEmote.Add(record);
                CurrentEmote = record;
            }
            else if (CurrentTable == Table.EmoteAction)
            {
                var record = new WeeniePropertiesEmoteAction();
                PopulateFields(record, fields);
                CurrentEmote.WeeniePropertiesEmoteAction.Add(record);
            }
            else if (CurrentTable == Table.EventFilter)
            {
                var record = new WeeniePropertiesEventFilter();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesEventFilter.Add(record);
            }
            else if (CurrentTable == Table.Float)
            {
                var record = new WeeniePropertiesFloat();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesFloat.Add(record);
            }
            else if (CurrentTable == Table.Generator)
            {
                var record = new WeeniePropertiesGenerator();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesGenerator.Add(record);
            }
            else if (CurrentTable == Table.IID)
            {
                var record = new WeeniePropertiesIID();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesIID.Add(record);
            }
            else if (CurrentTable == Table.Int)
            {
                var record = new WeeniePropertiesInt();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesInt.Add(record);
            }
            else if (CurrentTable == Table.Int64)
            {
                var record = new WeeniePropertiesInt64();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesInt64.Add(record);
            }
            else if (CurrentTable == Table.Palette)
            {
                var record = new WeeniePropertiesPalette();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesPalette.Add(record);
            }
            else if (CurrentTable == Table.Position)
            {
                var record = new WeeniePropertiesPosition();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesPosition.Add(record);
            }
            else if (CurrentTable == Table.Skill)
            {
                var record = new WeeniePropertiesSkill();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesSkill.Add(record);
            }
            else if (CurrentTable == Table.SpellBook)
            {
                var record = new WeeniePropertiesSpellBook();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesSpellBook.Add(record);
            }
            else if (CurrentTable == Table.String)
            {
                var record = new WeeniePropertiesString();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesString.Add(record);
            }
            else if (CurrentTable == Table.TextureMap)
            {
                var record = new WeeniePropertiesTextureMap();
                PopulateFields(record, fields);
                Weenie.WeeniePropertiesTextureMap.Add(record);
            }
        }

        public void PopulateFields(object record, List<string> fields)
        {
            for (var i = 0; i < CurrentColumns.Count; i++)
            {
                if (fields[i] == "NULL")
                    continue;
                
                var column = CurrentColumns[i];
                var propName = GetPropertyName(column);
                var prop = record.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                {
                    Console.WriteLine($"AddRecord: couldn't find propName {propName} in WeeniePropertiesInt");
                    continue;
                }
                var value = GetValueType(prop, fields[i]);
                prop.SetValue(record, value, null);
            }
        }

        public static object GetValueType(PropertyInfo prop, string value)
        {
            if (prop.PropertyType.FullName.Contains("UInt32"))
            {
                if (!uint.TryParse(value, out var result))
                {
                    if (value.StartsWith("0x"))
                    {
                        value = value.Replace("0x", "");
                        if (!uint.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
                            Console.WriteLine($"Failed to parse {value} into UInt32");
                    }
                    else
                        Console.WriteLine($"Failed to parse {value} into UInt32");
                }
                return result;

            }
            else if (prop.PropertyType.FullName.Contains("Int32"))
            {
                if (!int.TryParse(value, out var result))
                    Console.WriteLine($"Failed to parse {value} into Int32");

                return result;

            }
            else if (prop.PropertyType.FullName.Contains("String"))
                return value;

            else if (prop.PropertyType.FullName.Contains("UInt64"))
            {
                if (!ulong.TryParse(value, out var result))
                    Console.WriteLine($"Failed to parse {value} into UInt64");

                return result;
            }
            else if (prop.PropertyType.FullName.Contains("Int64"))
            {
                if (!long.TryParse(value, out var result))
                    Console.WriteLine($"Failed to parse {value} into Int64");

                return result;
            }
            else if (prop.PropertyType.FullName.Contains("UInt16"))
            {
                if (!ushort.TryParse(value, out var result))
                    Console.WriteLine($"Failed to parse {value} into UInt16");

                return result;
            }
            else if (prop.PropertyType.FullName.Contains("Int16"))
            {
                if (!short.TryParse(value, out var result))
                    Console.WriteLine($"Failed to parse {value} into Int16");

                return result;
            }
            else if (prop.PropertyType.FullName.Contains("DateTime"))
            {
                if (!DateTime.TryParse(value, out var result))
                    Console.WriteLine($"Failed to parse {value} into DateTime");

                return result;
            }
            else if (prop.PropertyType.FullName.Contains("Boolean"))
            {
                if (!bool.TryParse(value, out var result))
                    Console.WriteLine($"Failed to parse {value} into Boolean");

                return result;
            }
            else if (prop.PropertyType.FullName.Contains("Single"))
            {
                if (!float.TryParse(value, out var result))
                    Console.WriteLine($"Failed to parse {value} into Float");

                return result;
            }
            else if (prop.PropertyType.FullName.Contains("Double"))
            {
                if (!double.TryParse(value, out var result))
                    Console.WriteLine($"Failed to parse {value} into Double");

                return result;
            }
            else if (prop.PropertyType.FullName.Contains("SByte"))
            {
                if (!sbyte.TryParse(value, out var result))
                    Console.WriteLine($"Failed to parse {value} into SByte");

                return result;
            }
            else if (prop.PropertyType.FullName.Contains("Byte"))
            {
                if (!byte.TryParse(value, out var result))
                    Console.WriteLine($"Failed to parse {value} into Byte");

                return result;
            }

            Console.WriteLine($"Unknown property type: {prop.PropertyType.Name}");
            
            return value;
        }

        public static string GetPropertyName(string column)
        {
            var words = column.Split('_');
            var result = "";
            foreach (var word in words)
                result += word[0].ToString().ToUpper()[0] + word.Substring(1);

            return result;
        }

        public static Table GetTable(string table)
        {
            if (table.Equals("weenie"))
                return Table.Weenie;
            else if (table.Equals("weenie_properties_anim_part"))
                return Table.AnimPart;
            else if (table.Equals("weenie_properties_attribute"))
                return Table.Attribute;
            else if (table.Equals("weenie_properties_attribute_2nd"))
                return Table.Attribute2nd;
            else if (table.Equals("weenie_properties_body_part"))
                return Table.BodyPart;
            else if (table.Equals("weenie_properties_book"))
                return Table.Book;
            else if (table.Equals("weenie_properties_book_page_data"))
                return Table.BookPageData;
            else if (table.Equals("weenie_properties_bool"))
                return Table.Bool;
            else if (table.Equals("weenie_properties_create_list"))
                return Table.CreateList;
            else if (table.Equals("weenie_properties_d_i_d"))
                return Table.DID;
            else if (table.Equals("weenie_properties_emote"))
                return Table.Emote;
            else if (table.Equals("weenie_properties_emote_action"))
                return Table.EmoteAction;
            else if (table.Equals("weenie_properties_event_filter"))
                return Table.EventFilter;
            else if (table.Equals("weenie_properties_float"))
                return Table.Float;
            else if (table.Equals("weenie_properties_generator"))
                return Table.Generator;
            else if (table.Equals("weenie_properties_int"))
                return Table.Int;
            else if (table.Equals("weenie_properties_int64"))
                return Table.Int64;
            else if (table.Equals("weenie_properties_i_i_d"))
                return Table.IID;
            else if (table.Equals("weenie_properties_palette"))
                return Table.Palette;
            else if (table.Equals("weenie_properties_position"))
                return Table.Position;
            else if (table.Equals("weenie_properties_skill"))
                return Table.Skill;
            else if (table.Equals("weenie_properties_spell_book"))
                return Table.SpellBook;
            else if (table.Equals("weenie_properties_string"))
                return Table.String;
            else if (table.Equals("weenie_properties_texture_map"))
                return Table.TextureMap;
            else
                return Table.None;
        }

        public static List<string> GetColumns(string line)
        {
            var startIdx = line.IndexOf('`', line.IndexOf('('));
            var columns = new List<string>();
            while (startIdx != -1)
            {
                var endIdx = line.IndexOf('`', startIdx + 1);
                if (endIdx == -1)
                {
                    Console.WriteLine($"GetColumns({line}): couldn't find end delimiter after column {startIdx}");
                    break;
                }
                columns.Add(line.Substring(startIdx + 1, endIdx - startIdx - 1));
                startIdx = line.IndexOf('`', endIdx + 1);
            }
            return columns;
        }

        public static List<string> GetFields(string line)
        {
            line = RemoveComments(line);

            var startIdx = line.IndexOf('(') + 1;
            var fields = new List<string>();
            bool done = false;

            while (startIdx != -1)
            {
                if (startIdx >= line.Length) break;

                bool isString = line[startIdx] == '\'';

                var endIdx = -1;
                if (!isString)
                {
                    endIdx = line.IndexOf(',', startIdx + 1);
                    if (endIdx == -1)
                    {
                        endIdx = line.IndexOf(')', startIdx + 1);
                        if (endIdx == -1)
                            endIdx = line.Length;

                        done = true;
                    }
                }
                else
                {
                    var idx = startIdx + 1;
                    while (true)
                    {
                        endIdx = line.IndexOf('\'', idx);
                        if (endIdx == -1)
                        {
                            line = line + "'";
                            endIdx = line.Length;
                            break;
                        }
                        else if (line[endIdx - 1] == '\\')
                        {
                            idx = endIdx + 1;
                            continue;
                        }
                        endIdx++;
                        break;
                    }
                }

                if (endIdx == -1)
                {
                    Console.WriteLine($"GetFields({line}): couldn't find end delimiter after column {startIdx}");
                    break;
                }
                var field = line.Substring(startIdx, endIdx - startIdx).Trim();
                if (field.StartsWith("'") && field.EndsWith("'"))
                    field = field.Substring(1, field.Length - 2);

                fields.Add(field);
                startIdx = endIdx + 2;

                if (done) break;
            }
            return fields;
        }

        public static string RemoveComments(string line)
        {
            var startIdx = line.IndexOf("/*");
            while (startIdx != -1)
            {
                var endIdx = line.IndexOf("*/", startIdx + 1);
                if (endIdx == -1)
                {
                    Console.WriteLine($"RemoveComments({line}): couldn't find end delimiter after column {startIdx}");
                    break;
                }
                line = line.Substring(0, startIdx) + line.Substring(endIdx + 2);
                startIdx = line.IndexOf("/*");
            }
            return line;
        }
    }
}
