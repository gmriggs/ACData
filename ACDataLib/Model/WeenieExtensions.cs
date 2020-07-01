using System.Linq;

using ACE.Entity.Enum.Properties;

namespace ACE.Database.Models.World
{
    public static class WeenieExtensions
    {
        public static uint? GetProperty(this Weenie weenie, PropertyDataId property)
        {
            return weenie.WeeniePropertiesDID.FirstOrDefault(x => x.Type == (uint)property)?.Value;
        }
    }
}
