using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public enum Character{
        Nahida, Amber
    }
    public enum UnitType{
        Melee, Range, Heal
    }
    public enum BlockType
    {
        Plain, Hills, Mountains, Desert, Empty,
    }
    public enum Element
    {
        Pyro, Cryo, Hydro, Electro, Geo, Dendro, Anemo,
    }
    public enum Faction
    {
        Friendly, Neutral, Hostile
    }
    public enum BuildingType
    {
        Statue, Church
    }
    public enum RelicType
    {
        R0, R1
    }
}
