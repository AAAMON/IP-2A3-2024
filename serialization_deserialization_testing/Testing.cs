using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library;
using dune_library.Player_Resources;

namespace serialization_deserialization_testing
{
    internal class Testing
    {
        public static void Main(string[] args)
        {
            HashSet<Player> players = [new("0"), new("1"), new("2"), new("3"), new("4"), new("5")];
            Game.Start(players);

        }
    }
}
