using System;

namespace ACE.Entity.Enum
{
    [Flags]
    public enum DamageType
    {
        Undef       = 0x0,
        Slash       = 0x1,
        Pierce      = 0x2,
        Bludgeon    = 0x4,
        Cold        = 0x8,
        Fire        = 0x10,
        Acid        = 0x20,
        Electric    = 0x40,
        Health      = 0x80,
        Stamina     = 0x100,
        Mana        = 0x200,
        Nether      = 0x400,
        Base        = 0x10000000
    };
}
