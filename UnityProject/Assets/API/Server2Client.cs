using System.Collections;
using System.Collections.Generic;

namespace Nano
{
    public class NetUnit
    {
        // int row, col, strength, defence, life, range, exp, element, level;
        public string ID;
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
        public string ID;
        public int Row, Col;
        public BuildingType Type;
        public Faction Faction;
    }
    public class NetRelic
    {
        public string ID;
        public RelicType Type;
    }
    public class ServerEndGame : INetMessage
    {
        public bool Win;
    }
    public class ServerSetMap : INetMessage
    {
        public int Row, Col;
        public NetBlock[] Blocks;
    }
    public class ServerSetUnits : INetMessage
    {
        public NetUnit[] Units;
    }
    public class ServerSetRelics : INetMessage
    {
        public NetRelic[] Relics;
    }
    public class ServerSetBuildings : INetMessage
    {
        public NetBuilding[] Buildings;
    }

    public class ClientPrint : INetMessage
    {
        public string content;
    }
    //not used
}
