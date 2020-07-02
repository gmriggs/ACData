using System.IO;

using Newtonsoft.Json;

namespace ACE.Adapter.GDLE
{
    public class GDLELoader
    {
        public static bool TryLoadLandblock(string file, out Models.Landblock result)
        {
            try
            {
                var fileText = File.ReadAllText(file);

                result = JsonConvert.DeserializeObject<Models.Landblock>(fileText);

                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static bool TryLoadLandblock(string[] lines, out Models.Landblock result)
        {
            try
            {
                result = JsonConvert.DeserializeObject <Models.Landblock>(string.Join("\n", lines));

                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static bool TryLoadRecipeCombined(string file, out Models.RecipeCombined result)
        {
            try
            {
                var fileText = File.ReadAllText(file);

                result = JsonConvert.DeserializeObject<Models.RecipeCombined>(fileText);

                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static bool TryLoadRecipeCombined(string[] lines, out Models.RecipeCombined result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<Models.RecipeCombined>(string.Join("\n", lines));

                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static bool TryLoadQuest(string file, out Models.Quest result)
        {
            try
            {
                var fileText = File.ReadAllText(file);

                result = JsonConvert.DeserializeObject<Models.Quest>(fileText);

                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static bool TryLoadQuest(string[] lines, out Models.Quest result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<Models.Quest>(string.Join("\n", lines));

                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

    }
}
