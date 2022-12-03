using System.Collections;
using System.Collections.Generic;

namespace Nano
{
    public class NetUnit
    {
        public string ID;
        public Character Character;
        public UnitType Type;
        public int Row, Col;
        public int Strength, Defence, Life, Range, Speed;
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

    public class NetSaveInfo
    {
        public string Name;
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
    public class ServerSetBuildings : INetMessage
    {
        public NetBuilding[] Buildings;
    }

    public class ClientPrint : INetMessage
    {
        public string content;
    }

    public class NetSetSaveInfo : INetMessage
    {
        public NetSaveInfo[] SaveInfoList;
    }
}
