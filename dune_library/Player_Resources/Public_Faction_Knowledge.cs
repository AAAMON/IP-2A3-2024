using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace dune_library.Player_Resources {
  public class Public_Faction_Knowledge {
    public Faction Faction { get; }

    public uint Spice { get; private set; }

    public void Add_Spice(uint spice) => Spice += spice;

    public void Remove_Spice(uint spice) {
      if (Spice < spice) {
        throw new ArgumentException("Cannot remove more spice than the player has (current spice: " + Spice + ", to remove: " + spice + ")", nameof(spice));
      }
      Spice -= spice;
    }

    public uint Player_Marker { get; }

    public uint Reserves { get; }

    public uint Dead_Troops { get; }

    [JsonConstructor]
    public Public_Faction_Knowledge(Faction faction, uint spice, uint player_Marker, uint reserves) {
      Faction = faction;
      Spice = spice;
      Player_Marker = player_Marker;
      Reserves = reserves;
      Dead_Troops = 0;
    }
  }
}
