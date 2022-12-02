using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nano
{
    public interface INetMessage
    {

    }
    public enum Character
    {
        Skadi,Amiya
    }
    public enum UnitType
    {
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
        Blue, Neutral, Red
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
