﻿using System.IO;

namespace ACData
{
    public class JsonReader
    {
        public static ContentType GetContentType(string filename)
        {
            if (!File.Exists(filename))
                return ContentType.Undefined;

            // can possibly be indented format
            var json = File.ReadAllText(filename);

            if (json.Contains("\"weenies\":"))
                return ContentType.Landblock;
            if (json.Contains("\"wcid\":"))
                return ContentType.Weenie;
            else
                return ContentType.Undefined;
        }
    }
}