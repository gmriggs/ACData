using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Reflection;

namespace ACDataLib
{
    public class SQLReader
    {
        protected Table CurrentTable;

        protected List<string> CurrentColumns;

        protected int LastInsertId = -1;

        public static ContentType GetContentType(string filename)
        {
            if (!File.Exists(filename))
                return ContentType.Undefined;

            using (var streamReader = new StreamReader(filename))
            {
                var line = streamReader.ReadLine();

                while (line != null)
                {
                    if (line.Trim().Length == 0 || line.StartsWith("/*"))
                    {
                        line = streamReader.ReadLine();
                        continue;
                    }
                    return ProcessLine(line);
                }
                return ContentType.Undefined;
            }
        }

        public static ContentType ProcessLine(string line)
        {
            if (line.Contains("`landblock_instance`"))
                return ContentType.Landblock;
            else if (line.Contains("`quest`"))
                return ContentType.Quest;
            else if (line.Contains("`recipe`"))
                return ContentType.Recipe;
            else if (line.Contains("`weenie`"))
                return ContentType.Weenie;
            else
                return ContentType.Undefined;
        }

        public static ContentType GetContentType(string[] lines)
        {
            foreach (var line in lines)
            {
                if (line.Trim().Length == 0 || line.StartsWith("/*"))
                    continue;
                else
                    return ProcessLine(line);
            }
            return ContentType.Undefined;
        }

        protected static List<string> GetColumns(string line)
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

        protected static List<string> GetFields(string line)
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
                        else if (line[endIdx + 1] == '\'')
                        {
                            idx = endIdx + 2;
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
                
                // remove start and end quotes
                var field = line.Substring(startIdx, endIdx - startIdx).Trim();
                if (field.StartsWith("'") && field.EndsWith("'"))
                    field = field.Substring(1, field.Length - 2);

                // remove escape chars
                // verified: '''' produces ''
                field = field.Replace("''", "'");

                fields.Add(field);
                startIdx = endIdx + 2;

                if (done) break;
            }
            return fields;
        }

        protected static string RemoveComments(string line)
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

        protected void PopulateFields(object record, List<string> fields)
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
                    Console.WriteLine($"AddRecord: couldn't find propName {propName} in {record.GetType().Name}");
                    continue;
                }
                var value = GetValueType(prop, fields[i]);
                prop.SetValue(record, value, null);
            }
        }

        protected static string GetPropertyName(string column)
        {
            var words = column.Split('_');
            var result = "";
            foreach (var word in words)
                result += word[0].ToString().ToUpper()[0] + word.Substring(1).ToLower();

            return result;
        }

        protected object GetValueType(PropertyInfo prop, string value)
        {
            if (prop.PropertyType.FullName.Contains("UInt32"))
            {
                if (value.Equals("@parent_id"))
                    return (uint)LastInsertId;
                
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
                {
                    if (value.StartsWith("0x"))
                    {
                        value = value.Replace("0x", "");
                        if (!int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
                            Console.WriteLine($"Failed to parse {value} into Int32");
                    }
                    else
                        Console.WriteLine($"Failed to parse {value} into Int32");
                }
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
    }
}
