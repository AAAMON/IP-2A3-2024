﻿using dune_library.Map_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  internal abstract class Player_Info {
    public Player_Info(int spice,int reserves, int dead_forces, IList<General> generals,
      IList<General> traitors, IList<General> discarded_traitors) {
      Spice = spice;
      Reserves = reserves;
      Dead_Forces = dead_forces;
      Generals = generals;
      Traitors = traitors;
      Discarded_Traitors = discarded_traitors;
    }

    public Player_Info(Player_Info other) {
      Spice = other.Spice;
      Reserves = other.Reserves;
      Dead_Forces = other.Dead_Forces;
      Generals = other.Generals;
      Traitors = other.Traitors;
      Discarded_Traitors = other.Discarded_Traitors;
    }

    public int Spice { get; set; }
    
    public int Reserves { get; }

    public int Dead_Forces { get; set; }

    public IList<General> Generals { get; set; }

    public IList<General> Traitors { get; set; }

    public IList<General> Discarded_Traitors { get; set; }
  }
}
