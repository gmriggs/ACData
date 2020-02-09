using System.Collections.Generic;

using ACE.Database.Models.World;

namespace ACE.Adapter.GDLE
{
    public static class GDLEConverter
    {
        /// <summary>
        /// Converts ACE landblock instances -> GDLE landblock instances
        /// </summary>
        public static bool TryConvert(List<LandblockInstance> input, out Models.Landblock result)
        {
            result = new Models.Landblock();

            if (input.Count == 0)
                return true;

            result.key = (uint)input[0].Landblock << 16;

            result.value = new Models.LandblockValue();

            foreach (var lbi in input)
            {
                if (result.value.weenies == null)
                    result.value.weenies = new List<Models.LandblockWeenie>();

                var weenie = new Models.LandblockWeenie();
                weenie.id = lbi.Guid;

                // fix this ***, write it properly.
                var pos = new Models.Position();
                pos.ObjCellId = lbi.ObjCellId;

                var frame = new Models.Frame();

                frame.Origin = new Models.Origin();
                frame.Origin.X = lbi.OriginX;
                frame.Origin.Y = lbi.OriginY;
                frame.Origin.Z = lbi.OriginZ;

                frame.Angles = new Models.Angles();
                frame.Angles.W = lbi.AnglesW;
                frame.Angles.X = lbi.AnglesX;
                frame.Angles.Y = lbi.AnglesY;
                frame.Angles.Z = lbi.AnglesZ;

                pos.Frame = frame;
                weenie.pos = pos;

                weenie.wcid = lbi.WeenieClassId;

                result.value.weenies.Add(weenie);

                if (lbi.LandblockInstanceLink != null)
                {
                    foreach (var link in lbi.LandblockInstanceLink)
                    {
                        if (result.value.links == null)
                            result.value.links = new List<Models.LandblockLink>();

                        var _link = new Models.LandblockLink();
                        _link.source = link.ParentGuid;
                        _link.target = link.ChildGuid;

                        result.value.links.Add(_link);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Converts GDLE spawn map -> ACE landblock instances
        /// 
        /// This will not alter the Guid. To sanitize the Guid for ACE usage, you should use GDLELoader.TryLoadWorldSpawnsConverted() instead.
        /// </summary>
        public static bool TryConvert(Models.Landblock input, out List<LandblockInstance> results, out List<LandblockInstanceLink> links)
        {
            try
            {
                results = new List<LandblockInstance>();
                links = new List<LandblockInstanceLink>();

                foreach (var value in input.value.weenies)
                {
                    var result = new LandblockInstance();

                    result.Guid = value.id; // Collisions and other errors can be caused by invalid input. Data should be sanitized by the running ACE server.
                    //result.Landblock = input.key; ACE uses a virtual column here of (result.ObjCellId >> 16)
                    result.WeenieClassId = value.wcid;

                    result.ObjCellId = value.pos.ObjCellId;
                    result.OriginX = (float)value.pos.Frame.Origin.X;
                    result.OriginY = (float)value.pos.Frame.Origin.Y;
                    result.OriginZ = (float)value.pos.Frame.Origin.Z;
                    result.AnglesW = (float)value.pos.Frame.Angles.W;
                    result.AnglesX = (float)value.pos.Frame.Angles.X;
                    result.AnglesY = (float)value.pos.Frame.Angles.Y;
                    result.AnglesZ = (float)value.pos.Frame.Angles.Z;

                    results.Add(result);
                }

                if (input.value.links != null)
                {
                    foreach (var value in input.value.links)
                    {
                        var result = new LandblockInstanceLink();

                        result.ParentGuid = value.source;
                        result.ChildGuid = value.target;

                        links.Add(result);
                    }
                }

                return true;
            }
            catch
            {
                results = null;
                links = null;
                return false;
            }
        }
    }
}
