using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ACE.Database.Models.World;

namespace ACData
{
    public class RecipeSQLReader : SQLReader
    {
        private uint RecipeId;

        private Recipe Recipe;

        private List<CookBook> Cookbooks;

        private RecipeMod CurrentRecipeMod;

        public List<CookBook> ReadModel(string[] sqlLines)
        {
            Recipe = new Recipe();
            Cookbooks = new List<CookBook>();

            foreach (var line in sqlLines)
            {
                var match = Regex.Match(line, @"`id` = (\d+);");
                if (match.Success)
                {
                    uint.TryParse(match.Groups[1].Value, out RecipeId);
                    //Console.WriteLine($"Found recipe id {RecipeId}");
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

            return Cookbooks;
        }

        public void AddRecord(List<string> fields)
        {
            if (CurrentTable == Table.Recipe)
            {
                Recipe = new Recipe();
                PopulateFields(Recipe, fields);
            }
            else if (CurrentTable == Table.RecipeRequirementsBool)
            {
                var record = new RecipeRequirementsBool();
                PopulateFields(record, fields);
                Recipe.RecipeRequirementsBool.Add(record);
            }
            else if (CurrentTable == Table.RecipeRequirementsDID)
            {
                var record = new RecipeRequirementsDID();
                PopulateFields(record, fields);
                Recipe.RecipeRequirementsDID.Add(record);
            }
            else if (CurrentTable == Table.RecipeRequirementsFloat)
            {
                var record = new RecipeRequirementsFloat();
                PopulateFields(record, fields);
                Recipe.RecipeRequirementsFloat.Add(record);
            }
            else if (CurrentTable == Table.RecipeRequirementsIID)
            {
                var record = new RecipeRequirementsIID();
                PopulateFields(record, fields);
                Recipe.RecipeRequirementsIID.Add(record);
            }
            else if (CurrentTable == Table.RecipeRequirementsInt)
            {
                var record = new RecipeRequirementsInt();
                PopulateFields(record, fields);
                Recipe.RecipeRequirementsInt.Add(record);
            }
            else if (CurrentTable == Table.RecipeRequirementsString)
            {
                var record = new RecipeRequirementsString();
                PopulateFields(record, fields);
                Recipe.RecipeRequirementsString.Add(record);
            }
            else if (CurrentTable == Table.RecipeMod)
            {
                CurrentRecipeMod = new RecipeMod();
                PopulateFields(CurrentRecipeMod, fields);
                Recipe.RecipeMod.Add(CurrentRecipeMod);
            }
            else if (CurrentTable == Table.RecipeModsBool)
            {
                var record = new RecipeModsBool();
                PopulateFields(record, fields);
                CurrentRecipeMod.RecipeModsBool.Add(record);
            }
            else if (CurrentTable == Table.RecipeModsDID)
            {
                var record = new RecipeModsDID();
                PopulateFields(record, fields);
                CurrentRecipeMod.RecipeModsDID.Add(record);
            }
            else if (CurrentTable == Table.RecipeModsFloat)
            {
                var record = new RecipeModsFloat();
                PopulateFields(record, fields);
                CurrentRecipeMod.RecipeModsFloat.Add(record);
            }
            else if (CurrentTable == Table.RecipeModsIID)
            {
                var record = new RecipeModsIID();
                PopulateFields(record, fields);
                CurrentRecipeMod.RecipeModsIID.Add(record);
            }
            else if (CurrentTable == Table.RecipeModsInt)
            {
                var record = new RecipeModsInt();
                PopulateFields(record, fields);
                CurrentRecipeMod.RecipeModsInt.Add(record);
            }
            else if (CurrentTable == Table.RecipeModsString)
            {
                var record = new RecipeModsString();
                PopulateFields(record, fields);
                CurrentRecipeMod.RecipeModsString.Add(record);
            }
            else if (CurrentTable == Table.Cookbook)
            {
                var record = new CookBook();
                PopulateFields(record, fields);
                Cookbooks.Add(record);
                record.Recipe = Recipe;
            }
        }

        public static Table GetTable(string table)
        {
            if (table.Equals("recipe"))
                return Table.Recipe;
            else if (table.Equals("recipe_mod"))
                return Table.RecipeMod;
            else if (table.Equals("recipe_mods_bool"))
                return Table.RecipeModsBool;
            else if (table.Equals("recipe_mods_d_i_d"))
                return Table.RecipeModsDID;
            else if (table.Equals("recipe_mods_float"))
                return Table.RecipeModsFloat;
            else if (table.Equals("recipe_mods_int"))
                return Table.RecipeModsInt;
            else if (table.Equals("recipe_mods_i_i_d"))
                return Table.RecipeModsIID;
            else if (table.Equals("recipe_mods_string"))
                return Table.RecipeModsString;
            else if (table.Equals("recipe_requirements_bool"))
                return Table.RecipeRequirementsBool;
            else if (table.Equals("recipe_requirements_d_i_d"))
                return Table.RecipeRequirementsDID;
            else if (table.Equals("recipe_requirements_float"))
                return Table.RecipeRequirementsFloat;
            else if (table.Equals("recipe_requirements_int"))
                return Table.RecipeRequirementsInt;
            else if (table.Equals("recipe_requirements_i_i_d"))
                return Table.RecipeRequirementsIID;
            else if (table.Equals("recipe_requirements_string"))
                return Table.RecipeRequirementsString;
            else if (table.Equals("cook_book"))
                return Table.Cookbook;
            else
                return Table.None;
        }
    }
}
