﻿using dune_library.Spice;
using dune_library.Treachery_Cards;
using dune_library.Map_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources
{
  internal class Player
  {
    private Faction Faction { get; set; }
      
    private string Name { get; set; }
      
    private bool IsHuman { get; set; } //human or AI
      
    private bool IsActive { get; set; } //his turn
      
    private List<Treachery_Card> TreacheryCards { get; set; }
      
    private List<Spice_Card> SpiceCards { get; set; }
      
    public object Hand { get; private set; }
      
    private int ReserveTroops { get; set; } // New attribute for reserve troops
      
    public int Assigned_Sector { get; }

    public Player(string name, Faction faction, bool isHuman, ushort assigned_sector)
    {
      Name = name;
      Faction = faction;
      IsHuman = isHuman;
      TreacheryCards = new List<Treachery_Card>();
      SpiceCards = new List<Spice_Card>();
      Assigned_Sector = assigned_sector;
      ReserveTroops = 0;
    }

    public void PlayCard(Treachery_Card card) {
      if (TreacheryCards.Contains(card)) {
        //card.Execute(this);       <-- add some code here
        //Hand.Remove(card);
      } else {
        Console.WriteLine("Card not found.");
      }
    }

    public void ReviveGeneral(General general) {
      if (Faction.Graveyard.Generals.Contains(general) && Faction.Graveyard.CanReviveGenerals) {
        Faction.Graveyard.Generals.Remove(general);
        Faction.Generals.Add(general);
      } else {
        Console.WriteLine("Cannot revive the general.");
      }
    }

    public void MoveTroops(dune_library.Map.Section fromSection, dune_library.Map.Section toSection, int troopsCount) {
      if (Faction.TroopPositions.ContainsKey(fromSection) &&
          Faction.TroopPositions[fromSection] >= troopsCount) {
        Faction.TroopPositions[fromSection] -= troopsCount;
        Faction.TroopPositions[toSection] += troopsCount;
      } else {
        Console.WriteLine("Insufficient troops or incorrect section.");
      }
    }

    public void DeployTroops(dune_library.Map.Section toSection, int troopsCount) {
      if (ReserveTroops > troopsCount) {
        ReserveTroops -= troopsCount;
        if (Faction.TroopPositions.ContainsKey(toSection)) {
          Faction.TroopPositions[toSection] += troopsCount;
        } else {
          Faction.TroopPositions.Add(toSection, troopsCount);
        }
      } else {
        Console.WriteLine("Insufficient reserve troops.");
      }
    }
  }
}