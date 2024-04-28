﻿using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.UnsafeValueAccess;

namespace dune_library.Player_Resources {
  internal class Alliances_Manager {
    private ArgumentException Already_Has_An_Ally(Faction faction) =>
      new ArgumentException("This faction (" + faction + ") is already allied to another faction (" + Alliances[faction] + ")", nameof(faction));

    private ArgumentException Doesn_t_Have_An_Ally(Faction faction) =>
      new ArgumentException("This faction (" + faction + ") doesn't have an ally", nameof(faction));

    private ArgumentException Are_Not_Allied(Faction a, Faction b) =>
      new ArgumentException("These two factions (" + a + " and " + b + ") are not allied " +
        "(" + a + "'s ally: " + Alliances[a] + ", " + b + "'s ally: " + Alliances[b] + ")", nameof(a));

    private readonly IDictionary<Faction, Option<Faction>> Alliances;

    public Option<Faction> this[Faction faction] => Alliances[faction];

    public Alliances_Manager() {
      Alliances = new Dictionary<Faction, Option<Faction>> {
        [Faction.Atreides] = None,
        [Faction.Bene_Gesserit] = None,
        [Faction.Emperor] = None,
        [Faction.Fremen] = None,
        [Faction.Spacing_Guild] = None,
        [Faction.Harkonnen] = None,
      };
    }

    public Alliances_Manager(IDictionary<Faction, Option<Faction>> alliances) {
      Alliances = alliances;
    }

    public void Ally(Faction a, Faction b) {
      if (Alliances[a] != None) { throw Already_Has_An_Ally(a); }
      if (Alliances[b] != None) { throw Already_Has_An_Ally(b); }
      Alliances[a] = b;
      Alliances[b] = a;
    }

    public void Break_Alliance(Faction a, Faction b) {
      if (Alliances[a] == None) { throw Doesn_t_Have_An_Ally(a); }
      if (Alliances[b] == None) { throw Doesn_t_Have_An_Ally(b); }
      if (Alliances[a] != b || Alliances[b] != a) { throw Are_Not_Allied(a, b); }
      Alliances[a] = None;
      Alliances[b] = None;
    }

    public void Break_Alliance(Faction faction) {
      if (Alliances[faction] == None) { throw Doesn_t_Have_An_Ally(faction); }
      Faction former_ally = Alliances[faction].Value();
      Alliances[faction] = None;
      Alliances[former_ally] = None;
    }
  }
}
