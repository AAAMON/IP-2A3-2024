using dune_library.Decks.Spice;
using dune_library.Map_Resources;
using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt.Pipes;
using LanguageExt.SomeHelp;
using System;
using System.Linq;
using System.Security.Policy;
using static System.Collections.Specialized.BitVector32;

namespace dune_library.Phases {
  internal class Spice_Blow_And_Nexus : Phase
  {

        public override string name => "Spice Blow and Nexus";

        internal Spice_Deck Spice_Deck { get; }

        public override string moment { get; protected set; } = "";

        private uint turn;

        private I_Perspective_Generator Perspective_Generator { get; }

        private IReadOnlySet<Player> Players { get; }

        private I_Setup_Initializers_And_Getters Init { get; }

        public I_Input_Provider Input_Provider { get; set; }

        public bool[] Factions_To_Move { get; }

        private Tleilaxu_Tanks Tleilaxu_Tanks { get; }

        public Map_Resources.Map Map { get; }
        public Spice_Blow_And_Nexus(Game game)
      {
          Spice_Deck = game.Spice_Deck;
          turn = game.Round;
          Players = game.Players;
          Perspective_Generator = game;
          Init = game;
          Factions_To_Move = game.Factions_To_Move;
          Input_Provider = game.Input_Provider;
          Map = game.Map;
          Tleilaxu_Tanks = game.Tleilaxu_Tanks;
      }

      public override void Play_Out()
      {
          moment = "before choosing a spice card";
          Spice_Card topCard = Spice_Deck.Take_Next_Card();
          moment = "spice card was chosen";
          while (topCard is Shai_Hulud_Card && turn == 1) { 
              topCard = Spice_Deck.Take_Next_Card();
          }
        
          if (topCard is Shai_Hulud_Card)
          {
              HandleShaiHuludCard(topCard);
              while (topCard is Shai_Hulud_Card)
              {
                  topCard = Spice_Deck.Take_Next_Card();
              }
              HandleTerritoryCard((Territory_Card)topCard);
          }
          else if (topCard is Territory_Card)
          {
              HandleTerritoryCard((Territory_Card)topCard);
          }
          else
          {
              Console.WriteLine("Unknown card type drawn from Spice Deck.");
          }
          for(int i = 0; i < Factions_To_Move.Length; i++) { Factions_To_Move[i] = false; }
      }

      private void HandleShaiHuludCard(Spice_Card card)
      {
          moment = "Clearing the section";
          Territory_Card previous_card = (Territory_Card)Spice_Deck.Top_OF_Discard_Pile;

          Map.To_Section_With_Spice(previous_card.Section_Position_In_List).Delete_Spice();
          Map.To_Section_With_Spice(previous_card.Section_Position_In_List).Forces.Remove_By_Storm(Tleilaxu_Tanks);
          
          moment = "Waiting for Nexus Phase...";
          IList<(bool, Faction)> faction_responses = new List<(bool, Faction)>();
          Init.Factions_Distribution.Factions_In_Play.ForEach(faction =>
          {
              Init.Alliances.Break_Alliance(faction);
              switch (faction)
              {
                  case Faction.Atreides:
                      faction_responses.Add((true, Faction.Atreides));
                      Factions_To_Move[0] = true;
                      break;
                  case Faction.Bene_Gesserit:
                      faction_responses.Add((true,Faction.Bene_Gesserit));
                      Factions_To_Move[1] = true;
                      break;
                  case Faction.Emperor:
                      faction_responses.Add(((true, Faction.Emperor)));
                      Factions_To_Move[2] = true;
                      break;
                  case Faction.Fremen:
                      faction_responses.Add((true, Faction.Fremen));
                      Factions_To_Move[3] = true;
                      break;
                  case Faction.Spacing_Guild:
                      faction_responses.Add((true, Faction.Spacing_Guild));
                      Factions_To_Move[4] = true;
                      break;
                  case Faction.Harkonnen:
                      faction_responses.Add((true,Faction.Harkonnen));
                      Factions_To_Move[5] = true;
                      break;
              }
          });
          Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
          
          while(faction_responses.Length() > 0)
          {
                Console.WriteLine("/1/phase_2_input/2");
                string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                bool correct = false;
                Init.Factions_Distribution.Factions_In_Play.ForEach((faction) => {
                    if (line[1] == Init.Factions_Distribution.Player_Of(faction).Id && line[2] == "phase_2_input" && faction_responses.Contains((true, faction)))
                    {
                        if (line[3] == "pass")
                        {
                            faction_responses.Remove((true, faction));
                            switch (faction)
                            {
                                case Faction.Atreides:
                                    Factions_To_Move[0] = false;
                                    break;
                                case Faction.Bene_Gesserit:
                                    Factions_To_Move[1] = false;
                                    break;
                                case Faction.Emperor:
                                    Factions_To_Move[2] = false;
                                    break;
                                case Faction.Fremen:
                                    Factions_To_Move[3] = false;
                                    break;
                                case Faction.Spacing_Guild:
                                    Factions_To_Move[4] = false;
                                    break;
                                case Faction.Harkonnen:
                                    Factions_To_Move[5] = false;
                                    break;
                            }
                            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
                            correct = true;
                        }
                        else
                        {
                            Init.Factions_Distribution.Factions_In_Play.ForEach(faction2 => {
                                if (line[3] == Init.Factions_Distribution.Player_Of(faction2).Id && faction_responses.Contains((true, faction2)))
                                {
                                    if(faction != faction2)
                                    {

                                        faction_responses.Remove((true, faction));
                                        faction_responses.Remove((true, faction2));
                                        switch (faction)
                                        {
                                            case Faction.Atreides:
                                                Factions_To_Move[0] = false;
                                                break;
                                            case Faction.Bene_Gesserit:
                                                Factions_To_Move[1] = false;
                                                break;
                                            case Faction.Emperor:
                                                Factions_To_Move[2] = false;
                                                break;
                                            case Faction.Fremen:
                                                Factions_To_Move[3] = false;
                                                break;
                                            case Faction.Spacing_Guild:
                                                Factions_To_Move[4] = false;
                                                break;
                                            case Faction.Harkonnen:
                                                Factions_To_Move[5] = false;
                                                break;
                                        }
                                        switch (faction2)
                                        {
                                            case Faction.Atreides:
                                                Factions_To_Move[0] = false;
                                                break;
                                            case Faction.Bene_Gesserit:
                                                Factions_To_Move[1] = false;
                                                break;
                                            case Faction.Emperor:
                                                Factions_To_Move[2] = false;
                                                break;
                                            case Faction.Fremen:
                                                Factions_To_Move[3] = false;
                                                break;
                                            case Faction.Spacing_Guild:
                                                Factions_To_Move[4] = false;
                                                break;
                                            case Faction.Harkonnen:
                                                Factions_To_Move[5] = false;
                                                break;
                                        }
                                        Init.Alliances.Ally(faction, faction2);
                                        
                                        Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
                                        correct = true;
                                    }
                                }
                            });
                        }
                        
                    }
                    

                });

                if (!correct)
                {
                    Console.WriteLine("Failure");
                }
            }
          
      }

      private void HandleTerritoryCard(Territory_Card card)
      {
          
          if(Map.To_Section_With_Spice(card.Section_Position_In_List).Origin_Sector == Map.Storm_Sector) {
              Console.WriteLine("The Spice Blow icon is currently in the storm. No spice is placed this turn.");
              return;
          }
          Map.To_Section_With_Spice(card.Section_Position_In_List).Add_Spice();
          moment = "Puting spice in sectors";
          Console.WriteLine($"Spice Blow in sector {Map.To_Section_With_Spice(card.Section_Position_In_List).Id}. {Map.To_Section_With_Spice(card.Section_Position_In_List).Spice_Capacity} spice added to the territory.");

          Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
          }
  }
}
