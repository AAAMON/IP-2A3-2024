using dune_library.Map_Resources;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using LanguageExt.UnsafeValueAccess;
using System.Reflection.Metadata.Ecma335;

namespace dune_library.Player_Resources {
  public class General {
    [JsonIgnore]
    private static int counter = 0;

    public int Id { get; }

    public General(Faction faction, string name, int strength) {
      Id = counter++;
      Faction = faction;
      Name = name;
      Strength = strength;
      Location = None;
      Status = E_Status.Alive;
    }

    [JsonConstructor]
    public General(Faction faction, string name, int strength, int id, Option<Section> location, E_Status status) {
      Id = id;
      Faction = faction;
      Name = name;
      Strength = strength;
      Location = location;
      Status = status;
    }

    public Faction Faction { get; }

    public string Name { get; }

    public int Strength { get; }

    public Option<Section> Location { get; private set; }

    public bool Can_Be_In_Section(Section section) => Location.Match(
      None: true,
      Some: value => value == section
    );

    public void Place_In_Section(Section section) => Location = section;

    public void Remove_From_Section() => Location = None;

    #region Status Management

    public enum E_Status {
      Alive,
      Not_Revivable,
      Revivable,
    }

    private E_Status Status { get; set; }

    [JsonIgnore]
    public bool Is_Alive => Status == E_Status.Alive;

    [JsonIgnore]
    public bool Is_Dead => !Is_Alive;

    [JsonIgnore]
    public bool Is_Revivable => Status == E_Status.Revivable;

    [JsonIgnore]
    public bool Is_Not_Revivable => Status == E_Status.Not_Revivable;

    public void Kill() {
      if (Status == E_Status.Alive) {
        Status = E_Status.Not_Revivable;
      }
    }

    public void Make_Revivable() {
      if (Status == E_Status.Not_Revivable) {
        Status = E_Status.Revivable;
      }
    }

    public void Revive() {
      if (Status == E_Status.Revivable) {
        Status = E_Status.Alive;
      }
    }

    #endregion
  }
}
