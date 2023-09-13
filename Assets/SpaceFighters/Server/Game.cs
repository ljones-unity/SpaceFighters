using System.Collections.Generic;

#nullable enable

namespace SpaceFighters.Server
{
    public class Game
    {
        public Dictionary<ulong, Player> PlayerGameInfoTable = new();
    }
}
