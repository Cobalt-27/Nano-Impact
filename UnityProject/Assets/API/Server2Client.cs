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
        public int Level;
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
        public RelicType Type;
    }

    public class NetSaveInfo
    {
        public string Name;
        public string Description;
    }
    public class ServerStartGame : INetMessage
    {
        public GameMode GameMode;
        public Faction Faction;//invalid if singleplay
    }
    public class ServerEndGame : INetMessage
    {
        public Faction Winner;
    }
    public class ServerSetMap : INetMessage
    {
        public int Row, Col;
        public NetBlock[] Blocks;
    }
    public class PopMessage : INetMessage
    {
        public int Row,Col;
        public string Content;
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
        public string Content;
    }

    public class ClientInfo: INetMessage{
        public string Content;
    }

    public class ClientShow : INetMessage{
        public string Content;
    }

    public class NetSetSaveInfo : INetMessage
    {
        public NetSaveInfo[] SaveInfoList;
    }
    public class NetPlayAttack : INetMessage
    {
        public string ID;
    }
    public class NetPlaySound : INetMessage{
        string Name;
    }
}
