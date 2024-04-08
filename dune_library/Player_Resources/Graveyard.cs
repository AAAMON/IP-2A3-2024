using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources
{
    internal class Graveyard
    {
        public List<General> Generals { get; set; }
        public int Troops { get; set; }
        public bool CanReviveGenerals { get; set; }
    }
}
