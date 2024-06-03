using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Phases
{
    internal class Battle_Phase : Phase
    {
        public override string name => "Battle";

        public override string moment { get; protected set; } = "";

        public override void Play_Out()
        {
        }
    }
}
