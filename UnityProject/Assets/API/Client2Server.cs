using System.Collections;
using System.Collections.Generic;

namespace Nano
{
    public class ServerPrint : INetMessage
    {
        public string content;
    }
    public class StartGame : INetMessage
    {
        //null if load save
        public int MapRow, MapCol;
        public int Seed;
        //null if create new
        public bool Load;
        public string SaveName;
    }

    //unit
    public class Upgrade : INetMessage
    {
        public string ID;
    }

    public class Interact : INetMessage
    {
        public string From, To;
    }

    class Move : INetMessage
    {
        public string ID;
        public int Row, Col;
    }

    class SetRelic : INetMessage
    {
        public string ID;
        public string Relic;//null for remove
    }

    class AddBuilding : INetMessage
    {
        public BuildingType Type;
        public int Row, Col;
    }

    class EndRound : INetMessage
    {

    }

    class SaveGame : INetMessage
    {
        public string Name;
    }

    class Quit : INetMessage
    {

    }
}
