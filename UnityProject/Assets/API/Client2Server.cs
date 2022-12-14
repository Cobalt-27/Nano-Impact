using System.Collections;
using System.Collections.Generic;

namespace Nano
{
    class NetGreet : INetMessage
    {
        public string ClientName;
    }
    public class ServerPrint : INetMessage
    {
        public string content;
    }
    public class NetStartGame : INetMessage
    {
        public int MapRow, MapCol;//ignore
        public int Seed;//ignored
        public bool Load;//ignored
        public string SaveName;//"default"
    }
    public class NetInteract : INetMessage
    {
        public string From, To;
    }

    class NetMove : INetMessage
    {
        public string ID;
        public int Row, Col;
    }

    class NetAssignRelic : INetMessage
    {
        public string ID;
        public string Relic;//null for remove
    }

    class NetAddBuilding : INetMessage
    {
        public BuildingType Type;
        public int Row, Col;
    }

    class NetEndRound : INetMessage
    {

    }

    class NetSave : INetMessage
    {
        public string Name;
    }

    class NetQuit : INetMessage
    {

    }
    class NetRollback : INetMessage
    {

    }
    class NetSelectSave : INetMessage
    {
        public string Name;
    }
}
