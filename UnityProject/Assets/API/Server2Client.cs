using System.Collections;
using System.Collections.Generic;

namespace Nano
{
    public class NetUnit
    {
        // int row, col, strength, defence, life, range, exp, element, level;
        public Character Character;
        public UnitType Type;
        public int Row, Col;
        public int Strengh, Defence, Life, Range;
        public int Exp;
        public bool CanMove, CanAttack;
        public string RelicID;
        public Faction Faction;
    }
    public class NetBlock
    {
        public int Row, Col;
        public float Height;
        public BlockType Type;
    }
    public class NetBuilding
    {
        public int Row, Col;
        public BuildingType Type;
        public Faction Faction;
    }
    public class NetRelic
    {
        public RelicType Type;
    }
    public class EndGame : INetMessage
    {
        public bool Win;
    }
    public class SetMap : INetMessage
    {
        public int Row, Col;
        public NetBlock[] Blocks;
    }
    public class SetUnits : INetMessage
    {
        public NetUnit[] Units;
    }
    public class SetRelics : INetMessage
    {
        public NetRelic[] Relics;
    }
    public class SetBuildings : INetMessage
    {
        public NetBuilding[] Buildings;
    }

    public class Log : INetMessage
    {
        public string content;
    }
    //not used
}
