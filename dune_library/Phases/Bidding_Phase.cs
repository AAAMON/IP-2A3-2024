using System;
using System.Collections.Generic;
using System.Linq;
using dune_library.Player_Resources;
using dune_library.Utils;
using dune_library.Map_Resources;
using dune_library.Decks.Treachery;
using static dune_library.Utils.Exceptions;
using LanguageExt;
using static dune_library.Decks.Treachery.Treachery_Cards;
using dune_library.Player_Resources.Knowledge_Manager_Interfaces;
using Map = dune_library.Map_Resources.Map;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;
using LanguageExt.Pipes;

namespace dune_library.Phases
{
        internal class Bidding_Phase : Phase
    {
        private const int Max_Cards_Harkonnen = 8;
        private const int Max_Cards_Others = 4;

        public Bidding_Phase(Game game)
        {
            Perspective_Generator = game;
            Init = game;
            Players = game.Players;
            this.treachery_Deck = game.Treachery_Deck;
            HighestBid = game.HighestBid;
            Input_Provider = game.Input_Provider;
            this.Storm_Position = game.Map.Storm_Sector;
            Factions_To_Move = game.Factions_To_Move;
        }

        public bool[] Factions_To_Move { get; }

        public uint Storm_Position;
        public I_Input_Provider Input_Provider { get; set; }
        public override string name => "Bidding";

        public override string moment { get; protected set; } = "";

        private I_Perspective_Generator Perspective_Generator { get; }

        private I_Setup_Initializers_And_Getters Init { get; }

        private IReadOnlySet<Player> Players { get; }

        private IReadOnlySet<Faction> Factions_In_Play => Init.Factions_Distribution.Factions_In_Play;

        private Treachery_Deck treachery_Deck;

        private I_Treachery_Cards_Manager Treachery_Cards_Manager => Init.Knowledge_Manager;

        private Knowledge_Manager Faction_Knowledge => Init.Knowledge_Manager;

        private I_Spice_Manager Spice_Manager => Init.Knowledge_Manager;

        public Highest_Bid HighestBid;

        public List<Faction> GetFactionOrder()
        {

            // Convert positions to a list of factions sorted by their positions
            List<Faction> sortedFactions = new List<Faction>();
            Factions_In_Play.ForEach(faction => sortedFactions.Add(faction));

            // Find the starting point based on the given number
            int startIndex = 0;
            for (int i = 0; i < sortedFactions.Count; i++)
            {
                if (Init.Player_Markers.Marker_Of(sortedFactions[i]) > Storm_Position)
                {
                    startIndex = i;
                    break;
                }
            }

            // Generate the ordered list starting from the determined position
            List<Faction> result = new List<Faction>();
            for (int i = startIndex; i < sortedFactions.Count; i++)
            {
                result.Add(sortedFactions[i]);
            }
            for (int i = 0; i < startIndex; i++)
            {
                result.Add(sortedFactions[i]);
            }

            return result;
        }


        public override void Play_Out()
        {
            string Get_Card = Wait_Until_Something.AwaitInput(3000, Input_Provider).Result;
            Console.WriteLine(Get_Card);

            moment = "bidding declaration";

            List<Faction> faction_order = GetFactionOrder();

            Console.WriteLine(faction_order);

            int max_number_of_cards = 0;

            faction_order.ForEach(faction => {
                if (Faction_Knowledge.Of(faction).Number_Of_Treachery_Cards_Of(faction) < (faction == Faction.Harkonnen ? Max_Cards_Harkonnen : Max_Cards_Others))
                    max_number_of_cards++;
                });

            switch (faction_order.First())
            {
                case Faction.Atreides:
                    Factions_To_Move[0] = true;
                    break;
                case Faction.Bene_Gesserit:
                    Factions_To_Move[1] = true;
                    break;
                case Faction.Emperor:
                    Factions_To_Move[2] = true;
                    break;
                case Faction.Fremen:
                    Factions_To_Move[3] = true;
                    break;
                case Faction.Spacing_Guild:
                    Factions_To_Move[4] = true;
                    break;
                case Faction.Harkonnen:
                    Factions_To_Move[5] = true;
                    break;
            }

            bool[] who_passed = new bool[6]; //Atreides Bene Gesserit Emperor Fremen Guild Harkonnen

            moment = "bidding started";

            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

            while (max_number_of_cards > 0 && who_passed.Contains(false)) {
                var biddingOrder = new Queue<Faction>();
                faction_order.ForEach(faction => {
                    if (Faction_Knowledge.Of(faction).Number_Of_Treachery_Cards_Of(faction) < (faction == Faction.Harkonnen ? Max_Cards_Harkonnen : Max_Cards_Others))
                        biddingOrder.Enqueue(faction);
                });

                var topCard = treachery_Deck.Next_Card_Peek;

                while (biddingOrder.Any())
                {
                    var currentBidder = biddingOrder.Dequeue();

                    Console.WriteLine("Introduceti bid-ul (ex /player1/phase_4_input/3)");
                    string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                    bool correct = false;
                    if (line[1] == Init.Factions_Distribution.Player_Of(currentBidder).Id && line[2] == "phase_4_input")
                    {
                        if (line[3] == "pass")
                        {
                            switch (currentBidder)
                            {
                                case Faction.Atreides:
                                    who_passed[0] = true;
                                    break;
                                case Faction.Bene_Gesserit:
                                    who_passed[1] = true;
                                    break;
                                case Faction.Emperor:
                                    who_passed[2] = true;
                                    break;
                                case Faction.Fremen:
                                    who_passed[3] = true;
                                    break;
                                case Faction.Spacing_Guild:
                                    who_passed[4] = true;
                                    break;
                                case Faction.Harkonnen:
                                    who_passed[5] = true;
                                    break;
                            }
                            for (int i = 0; i < Factions_To_Move.Length; i++) { Factions_To_Move[i] = false; }
                            switch (currentBidder)
                            {
                                case Faction.Atreides:
                                    Factions_To_Move[1] = true;
                                    break;
                                case Faction.Bene_Gesserit:
                                    Factions_To_Move[2] = true;
                                    break;
                                case Faction.Emperor:
                                    Factions_To_Move[3] = true;
                                    break;
                                case Faction.Fremen:
                                    Factions_To_Move[4] = true;
                                    break;
                                case Faction.Spacing_Guild:
                                    Factions_To_Move[5] = true;
                                    break;
                                case Faction.Harkonnen:
                                    Factions_To_Move[0] = true;
                                    break;
                            }
                            biddingOrder.Enqueue(currentBidder);

                            correct = true;
                            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

                        }
                        else if(!HighestBid.faction.IsNone)
                        {
                            if(HighestBid.faction == currentBidder) {
                                break;
                            }
                        }
                        else
                        {
                            int bid = 0;
                            if (Int32.TryParse(line[3], out bid)) {
                                if (bid > HighestBid.bid && Spice_Manager.getSpice(currentBidder) >= bid)
                                {
                                    HighestBid.bid = (uint)bid;
                                    HighestBid.faction = currentBidder;
                                    for (int i = 0; i < Factions_To_Move.Length; i++) { Factions_To_Move[i] = false; }
                                    switch (currentBidder)
                                    {
                                        case Faction.Atreides:
                                            Factions_To_Move[1] = true;
                                            break;
                                        case Faction.Bene_Gesserit:
                                            Factions_To_Move[2] = true;
                                            break;
                                        case Faction.Emperor:
                                            Factions_To_Move[3] = true;
                                            break;
                                        case Faction.Fremen:
                                            Factions_To_Move[4] = true;
                                            break;
                                        case Faction.Spacing_Guild:
                                            Factions_To_Move[5] = true;
                                            break;
                                        case Faction.Harkonnen:
                                            Factions_To_Move[0] = true;
                                            break;
                                    }
                                    Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
                                    correct = true;
                                    who_passed = new bool[6];
                                }
                            }
                        }
                    }
                    if(!correct)
                    {
                        Console.WriteLine("Failure");
                    }
                }

                Console.WriteLine($"The winner is {HighestBid.faction}");
                if (who_passed.Contains(false) && HighestBid.bid != 0)
                {
                    if (HighestBid.faction == Faction.Harkonnen)
                    {
                        Treachery_Cards_Manager.Give_A_Treachery_Card((Faction)HighestBid.faction);
                    }

                    if (Factions_In_Play.Contains(Faction.Emperor) && HighestBid.faction != Faction.Emperor)
                    {
                        Spice_Manager.Add_Spice_To(Faction.Emperor, HighestBid.bid);
                    }

                    Treachery_Cards_Manager.Give_A_Treachery_Card((Faction)HighestBid.faction);
                    Spice_Manager.Remove_Spice_From((Faction)HighestBid.faction, HighestBid.bid);

                    HighestBid.bid = 0;

                    Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
                }
            }
            moment = "end of bidding";
            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
        }
    }
}
