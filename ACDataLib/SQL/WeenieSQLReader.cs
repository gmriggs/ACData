using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

using ACE.Database.Models.World;

namespace ACData
{
    public class WeenieSQLReader: SQLReader
    {
        private uint Wcid;

        private Weenie Weenie;

        private WeeniePropertiesEmote CurrentEmote;

        public Weenie ReadModel(string[] sqlLines)
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
                {
                    LastInsertId++;
                    continue;
                }

                if (line.Contains("("))
                {
                    var fields = GetFields(line);
                    //Console.WriteLine($"Found fields: {string.Join(", ", fields)}");

                    AddRecord(fields);
                }
            }
            return Weenie;
        }

        private void AddRecord(List<string> fields)
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

        private static Table GetTable(string table)
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
    }
}
