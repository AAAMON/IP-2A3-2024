using dune_library.Decks.Treachery;
using dune_library.Player_Resources.Knowledge_Manager_Interfaces;
using dune_library.Utils;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static dune_library.Decks.Treachery.Treachery_Cards;
using static dune_library.Player_Resources.Knowledge_Manager_Interfaces.I_Spice_Manager;
using static dune_library.Player_Resources.Knowledge_Manager_Interfaces.I_Treachery_Cards_Manager;
using static dune_library.Utils.Exceptions;

namespace dune_library.Player_Resources
{
    public class Knowledge_Manager : I_Knowledge_Manager {
    public Knowledge_Manager(IReadOnlySet<Faction> factions_in_play, Treachery_Deck treachery_deck) {
      Faction_Knowledge_Dict = factions_in_play.Select(faction =>
        new KeyValuePair<Faction, I_Faction_Knowledge>(faction, new Faction_Knowledge(factions_in_play))
      ).ToDictionary();
      Treachery_Deck = treachery_deck; 
    }

    private Treachery_Deck Treachery_Deck { get; }

    private IDictionary<Faction, I_Faction_Knowledge> Faction_Knowledge_Dict { get; }

    #region I_Knowledge_Manager_Read_Only implementation

    public I_Faction_Knowledge_Read_Only Of(Faction faction) {
      if (Faction_Knowledge_Dict.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      return Faction_Knowledge_Dict[faction];
    }

    #endregion

    #region I_Spice_Manager implementation

    public void Transfer_Spice_To(Faction sender, Option<Faction> reciever, uint to_transfer) {
      if (reciever.IsSome && Faction_Knowledge_Dict.ContainsKey(reciever.Value()) == false) {
        throw new Faction_Not_In_Play(reciever.Value());
      }
      if (Remove_Spice_From(sender, to_transfer) == false) {
        throw new Faction_Does_Not_Have_Enough_Spice(sender, Faction_Knowledge_Dict[sender].Spice, to_transfer);
      }
      if (reciever.IsNone) {
        return;
      }
      Add_Spice_To(reciever.Value(), to_transfer);
    }

    public void Add_Spice_To(Faction reciever, uint to_add) {
      if (Faction_Knowledge_Dict.ContainsKey(reciever) == false) {
        throw new Faction_Not_In_Play(reciever);
      }
      Faction_Knowledge_Dict[reciever].Add_Spice(to_add);
    }

    public bool Remove_Spice_From(Faction source, uint to_remove) {
      if (Faction_Knowledge_Dict.ContainsKey(source) == false) {
        throw new Faction_Not_In_Play(source);
      }
      return Faction_Knowledge_Dict[source].Remove_Spice(to_remove);
    }

    public uint getSpice(Faction source)
    {
       if (Faction_Knowledge_Dict.ContainsKey(source) == false) {
        throw new Faction_Not_In_Play(source);
      }
      return Faction_Knowledge_Dict[source].Spice;
    }

    #endregion

    #region I_Treachery_Cards_Manager implementation

    public void Give_A_Treachery_Card(Faction faction) {
      if (Faction_Knowledge_Dict.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      Faction_Knowledge_Dict[faction].Add_Treachery_Card(Treachery_Deck.Take_Next_Card());
      Faction_Knowledge_Dict.Keys.ForEach(to_update =>
        Faction_Knowledge_Dict[to_update].Add_To_Number_Of_Treachery_Cards_Of(faction)
      );
    }

    public void Remove_Treachery_Card(Faction faction, Treachery_Card to_remove) {
      if (Faction_Knowledge_Dict.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      if (Faction_Knowledge_Dict[faction].Remove_Treachery_Card(to_remove) == false) {
        throw new Faction_Does_Not_Own_This_Treachery_Card(faction, Faction_Knowledge_Dict[faction].Treachery_Cards, to_remove);
      }
      Treachery_Deck.Add_To_Discard_Pile(to_remove);
      Faction_Knowledge_Dict.Keys.ForEach(to_update =>
        Faction_Knowledge_Dict[to_update].Remove_From_Number_Of_Treachery_Cards_Of(faction) // always greater than zero
      );
    }

    #endregion

    #region I_Traitors_Initializer implementation

    public void Init_Traitors(Faction faction, IReadOnlyList<General> traitors, IReadOnlyList<General> discarded_traitors) {
      if (Faction_Knowledge_Dict.ContainsKey(faction) == false) {
        throw new Faction_Not_In_Play(faction);
      }
      Faction_Knowledge_Dict[faction].Init_Traitors(traitors, discarded_traitors);
    }

    #endregion
  }
}
