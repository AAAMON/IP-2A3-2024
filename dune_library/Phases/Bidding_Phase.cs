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

namespace dune_library.Phases
{
    internal class HighestBidder
    {
        public Faction Faction { get; set; }
        public uint bid { get; set; }

        public HighestBidder(Faction faction, uint bid)
        {
            this.Faction = faction;
            this.bid = bid;
        }
        public HighestBidder()
        {
            this.bid = 0;
        }
    }
        internal class Bidding_Phase : Phase
    {
        private const int Max_Cards_Harkonnen = 8;
        private const int Max_Cards_Others = 4;
        private const int Bid_Timeout = 10;

        public Bidding_Phase(
            I_Perspective_Generator perspective_generator,
            I_Setup_Initializers_And_Getters init,
            IReadOnlySet<Player> players,
            Treachery_Deck treachery_Deck)
        {
            Perspective_Generator = perspective_generator;
            Init = init;
            Players = players;
            this.treachery_Deck = treachery_Deck;
        }

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
        public override void Play_Out()
        {
            moment = "bidding declaration";

            var biddingQueue = new Queue<Faction>();
            Factions_In_Play.ForEach(faction => {
                if (Faction_Knowledge.Of(faction).Number_Of_Treachery_Cards_Of(faction) < (faction == Faction.Harkonnen ? Max_Cards_Harkonnen : Max_Cards_Others))
                    biddingQueue.Enqueue(faction);
                });
            var topCard = treachery_Deck.Next_Card_Peek;

            var biddingOrder = new Queue<Faction>(biddingQueue);

            uint counter = 0;

            bool stop = false;

            for(int i = 0; i < biddingOrder.Count && !stop; i++)
            {
                var highestBidder = new HighestBidder();

                while (biddingOrder.Any())
                {
                    var currentBidder = biddingOrder.Dequeue();
                    var bid = GetBidFromPlayer(currentBidder, highestBidder);
                    Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));


                    if (bid > highestBidder.bid)
                    {
                        highestBidder.bid = (uint)bid;
                        highestBidder.Faction = currentBidder;
                        counter = 0;
                    }
                    else
                    {
                        counter++;
                    }
                    if(counter >= biddingOrder.Count)
                    {
                        stop = true;
                        break;
                    }
                    if (Faction_Knowledge.Of(currentBidder).Number_Of_Treachery_Cards_Of(currentBidder) < (currentBidder == Faction.Harkonnen ? Max_Cards_Harkonnen : Max_Cards_Others))
                        biddingOrder.Enqueue(currentBidder);
                }
                Console.WriteLine($"The winner is {highestBidder.Faction}");
                if (highestBidder.bid != 0)
                {
                    if (highestBidder.Faction == Faction.Harkonnen)
                    {
                    
                        Treachery_Cards_Manager.Give_A_Treachery_Card(highestBidder.Faction);
                    }
                    if (Factions_In_Play.Contains(Faction.Emperor))
                    {
                        Spice_Manager.Add_Spice_To(Faction.Emperor, highestBidder.bid);
                        Spice_Manager.Remove_Spice_From(highestBidder.Faction, highestBidder.bid);
                    }
                    else
                    {
                        Spice_Manager.Remove_Spice_From(highestBidder.Faction, highestBidder.bid);
                    }
                }
            }
            moment = "end of bidding";
            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
        }

        private int GetBidFromPlayer(Faction faction, HighestBidder currentHighestBid)
        {
            Console.WriteLine($"{faction}, place your bid (current highest: {currentHighestBid.bid}) or pass:");
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out int bid))
                {
                    if (bid >= 0 && bid <= Spice_Manager.getSpice(faction))
                    {
                        return bid;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid bid. Please enter a valid bid between 0 and {Spice_Manager.getSpice(faction)}:");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number:");
                }
            }
        }
    }
}
