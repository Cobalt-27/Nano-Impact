using System.Collections;
using System.Collections.Generic;

namespace Nano
{
    public class ServerPrint : INetMessage
    {
        public string content;
    }
    public class NetStartGame : INetMessage
    {
        //null if load save
        public int MapRow, MapCol;
        public int Seed;
        //null if create new
        public bool Load;
        public string SaveName;
    }

    //unit
    public class NetUpgrade : INetMessage
    {
        public string ID;
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
}
