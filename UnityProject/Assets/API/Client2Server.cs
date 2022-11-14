using System.Collections;
using System.Collections.Generic;

namespace Nano
{
    public class ServerPrint{
        public string content;
    }
    public class StartGame
    {
        //null if load save
        public int MapRow, MapCol;
        public int Seed;
        //null if create new
        public bool Load;
        public string SaveName;
    }

    //unit
    public class Upgrade
    {
        public string ID;
    }

    public class Interact
    {
        public string From, To;
    }

    class Move
    {
        public string ID;
        public int Row, Col;
    }

    class SetRelic
    {
        public string ID;
        public string Relic;//null for remove
    }

    class AddBuilding
    {
        public BuildingType Type;
        public int Row, Col;
    }

    class EndRound
    {

    }

    class SaveGame
    {
        public string Name;
    }

    class Quit
    {

    }
}
