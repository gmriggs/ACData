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
    }
}
