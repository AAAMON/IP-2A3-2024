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
using LanguageExt.SomeHelp;

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
            treachery_Deck = game.Treachery_Deck;
            HighestBid = game.HighestBid;
            Input_Provider = game.Input_Provider;
            Storm_Position = game.Map.Storm_Sector;
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

        public void Next_Faction(Faction faction)
        {
            switch (faction)
            {
                case Faction.Atreides:
                    Factions_To_Move[0] = false;
                    Factions_To_Move[1] = true;
                    break;
                case Faction.Bene_Gesserit:
                    Factions_To_Move[1] = false;
                    Factions_To_Move[2] = true;
                    break;
                case Faction.Emperor:
                    Factions_To_Move[2] = false;
                    Factions_To_Move[3] = true;
                    break;
                case Faction.Fremen:
                    Factions_To_Move[3] = false;
                    Factions_To_Move[4] = true;
                    break;
                case Faction.Spacing_Guild:
                    Factions_To_Move[4] = false;
                    Factions_To_Move[5] = true;
                    break;
                case Faction.Harkonnen:
                    Factions_To_Move[5] = false;
                    Factions_To_Move[0] = true;
                    break;
            }
        }

        private bool[] Has_Passed(bool[] who_passed, Faction faction)
        {
            switch (faction)
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
            return who_passed;
        }

        private List<Faction> Initialize_Pass()
        {
            List<Faction> who_didnt_pass = new List<Faction>();
            List<Faction> faction_order = GetFactionOrder();
            faction_order.ForEach(faction => {
                if (Faction_Knowledge.Of(faction).Number_Of_Treachery_Cards_Of(faction) < (faction == Faction.Harkonnen ? Max_Cards_Harkonnen : Max_Cards_Others))
                {
                    who_didnt_pass.Add(faction);
                }
            });
            return who_didnt_pass;
        }

        public override void Play_Out()
        {
            //string Get_Card = Wait_Until_Something.AwaitInput(3000, Input_Provider).Result;
            //Console.WriteLine(Get_Card);

            moment = "bidding declaration";

            List<Faction> faction_order = GetFactionOrder();

            var biddingOrder = new Queue<Faction>();
            faction_order.ForEach(faction => {
                if (Faction_Knowledge.Of(faction).Number_Of_Treachery_Cards_Of(faction) < (faction == Faction.Harkonnen ? Max_Cards_Harkonnen : Max_Cards_Others))
                { 
                    biddingOrder.Enqueue(faction);
                }
            });
            

            List<Faction> who_didnt_pass = Initialize_Pass();

            moment = "bidding started";

            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));


            while(biddingOrder.Any() && who_didnt_pass.Any())
            {
                who_didnt_pass = Initialize_Pass();

                for (int i = 0; i < faction_order.Count; i++) { Factions_To_Move[i] = false; }

                var currentBidder = biddingOrder.Dequeue();

                if (Faction_Knowledge.Of(currentBidder).Number_Of_Treachery_Cards_Of(currentBidder) >= (currentBidder == Faction.Harkonnen ? Max_Cards_Harkonnen : Max_Cards_Others))
                {
                    currentBidder = biddingOrder.Dequeue();
                }

                switch (currentBidder)
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
                Queue<Faction> currentBidding = new Queue<Faction>();
                biddingOrder.ForEach(currentBidding.Enqueue);

                biddingOrder.Enqueue(currentBidder);

                who_didnt_pass.ForEach(f => Console.WriteLine(f));

                Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

                

                while (who_didnt_pass.Any())
                {
                    Console.WriteLine($"Introduceti bid-ul (ex /player1/phase_4_input/3)");
                    Console.WriteLine(currentBidder);

                    Console.WriteLine();
                    who_didnt_pass.ForEach(f => Console.WriteLine(f));
                    Console.WriteLine();

                    string[] line = Input_Provider.GetInputAsync().Result.Split("/");
                    bool correct = false;

                    if (line[1] == Init.Factions_Distribution.Player_Of(currentBidder).Id && line[2] == "phase_4_input")
                    {
                        if (line[3] == "pass")
                        {
                            who_didnt_pass.Remove(currentBidder);
                            correct = true;
                        }
                        else
                        {
                            int bid = 0;
                            if (Int32.TryParse(line[3], out bid))
                            {
                                if (bid > HighestBid.bid && Spice_Manager.getSpice(currentBidder) >= bid)
                                {
                                    HighestBid.bid = (uint)bid;
                                    HighestBid.faction = currentBidder;

                                    correct = true;
                                    who_didnt_pass = Initialize_Pass();
                                }
                            }
                        }
                    }
                    if (!correct)
                    {
                        Console.WriteLine("Failure");
                    }
                    else
                    {

                        Next_Faction(currentBidder);

                        currentBidding.Enqueue(currentBidder);

                        currentBidder = currentBidding.Dequeue();

                        if (!HighestBid.faction.IsNone)
                        {
                            if ((Faction)HighestBid.faction == currentBidder)
                            {
                                break;
                            }
                        }

                        Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
                    }
                }

                if (who_didnt_pass.Any() && HighestBid.bid != 0)
                {
                    Console.WriteLine($"The winner is {HighestBid.faction}");
                    if (HighestBid.faction == Faction.Harkonnen && Init.Knowledge_Manager.Of(Faction.Harkonnen).Treachery_Cards.Count < 7)
                    {
                        Treachery_Cards_Manager.Give_A_Treachery_Card((Faction)HighestBid.faction);
                    }

                    if (Factions_In_Play.Contains(Faction.Emperor) && HighestBid.faction != Faction.Emperor)
                    {
                        Spice_Manager.Add_Spice_To(Faction.Emperor, HighestBid.bid);
                    }

                    Treachery_Cards_Manager.Give_A_Treachery_Card((Faction)HighestBid.faction);
                    Spice_Manager.Remove_Spice_From((Faction)HighestBid.faction, HighestBid.bid);

                    HighestBid = new Highest_Bid();

                }

            }

            moment = "end of bidding";
            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
        }
    }
}
