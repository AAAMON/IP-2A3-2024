using dune_library.Utils;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static dune_library.Utils.Exceptions;
using dune_library.Decks.Treachery;
using static dune_library.Decks.Treachery.Treachery_Cards;
using static dune_library.Player_Resources.I_Faction_Knowledge;

namespace dune_library.Player_Resources {
  public class Faction_Knowledge : I_Faction_Knowledge {

    public Faction_Knowledge(IReadOnlySet<Faction> factions_in_play) {
      traitors_are_initialized = false;
      treachery_cards = new Occurence_Dict<Treachery_Card>();
      traitors = None;
      discarded_traitors = None;
      number_of_treachery_cards_of_other_factions = factions_in_play.Select(faction
        => new KeyValuePair<Faction, uint>(faction, 0)
      ).ToDictionary();
      Spice = 0;
    }

    #region I_Faction_Knowledge_Read_Only implementation

    public uint Spice { get; private set; }

    private I_Occurence_Dict<Treachery_Card> treachery_cards { get; }
    public I_Occurence_Dict_Read_Only<Treachery_Card> Treachery_Cards => treachery_cards;

    private Option<IReadOnlyList<General>> traitors;
    public IReadOnlyList<General> Traitors {
      get => traitors.OrElseThrow(new Variable_Is_Not_Initialized(traitors));
      private set => traitors = Some(value);
    }

    private Option<IReadOnlyList<General>> discarded_traitors;
    public IReadOnlyList<General> Discarded_Traitors {
      get => discarded_traitors.OrElseThrow(new Variable_Is_Not_Initialized(discarded_traitors));
      private set => discarded_traitors = Some(value);
    }
    
    private Dictionary<Faction, uint> number_of_treachery_cards_of_other_factions { get; }
    public IReadOnlyDictionary<Faction, uint> Number_Of_Treachery_Cards_Of_Other_Factions { get => number_of_treachery_cards_of_other_factions; }

    #endregion

    #region Spice

    public void Add_Spice(uint to_add) => Spice += to_add;

    public bool Remove_Spice(uint to_remove) {
      if (Spice < to_remove) {
        return false;
      }
      Spice -= to_remove;
      return true;
    }

    #endregion

    #region Treachery Cards

    public void Add_Treachery_Card(Treachery_Card to_add) => treachery_cards.Add(to_add);

    public bool Remove_Treachery_Card(Treachery_Card to_remove) => treachery_cards.Remove(to_remove, 1);

    #endregion

    #region Traitors and Discarded Traitors

    private bool traitors_are_initialized;

    public void Init_Traitors(IReadOnlyList<General> traitors, IReadOnlyList<General> discarded_traitors) {
      if (traitors_are_initialized == true) {
        throw new Traitors_Have_Already_Been_Initialized();
      }
      Traitors = traitors;
      Discarded_Traitors = discarded_traitors;
      traitors_are_initialized = true;
    }

    #endregion

    #region Number of Treachery Cards Of Other Factions

    public uint Number_Of_Treachery_Cards_Of(Faction faction) {
      if (number_of_treachery_cards_of_other_factions.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      return number_of_treachery_cards_of_other_factions[faction];
    }

    public bool Number_Of_Treachery_Cards_Is_Not_Zero(Faction faction) =>
      Number_Of_Treachery_Cards_Of(faction) != 0;
    public void Add_To_Number_Of_Treachery_Cards_Of(Faction faction) {
      if (number_of_treachery_cards_of_other_factions.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      number_of_treachery_cards_of_other_factions[faction] += 1;
    }

    public bool Remove_From_Number_Of_Treachery_Cards_Of(Faction faction) {
      if (number_of_treachery_cards_of_other_factions.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      if (number_of_treachery_cards_of_other_factions[faction] == 0) {
        return false;
      }
      number_of_treachery_cards_of_other_factions[faction] -= 1;
      return true;
    }

    #endregion
  }
}
