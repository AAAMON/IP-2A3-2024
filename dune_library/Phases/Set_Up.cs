using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dune_library.Map_Resources;
using dune_library.Map_Resoures;
using System.Text.Json.Serialization;
using dune_library.Decks.Treachery;
using static dune_library.Utils.Exceptions;
using dune_library.Player_Resources.Knowledge_Manager_Interfaces;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static System.Collections.Specialized.BitVector32;
//using clientApi;

namespace dune_library.Phases
{
    internal class Set_Up : Phase
    {
        public Set_Up(
          I_Perspective_Generator perspective_generator,
          I_Setup_Initializers_And_Getters init,
          IReadOnlySet<Player> players,
          Either<Factions_Distribution_Manager, Final_Factions_Distribution> factions_distribution_raw,
          Option<Either<Player_Markers_Manager, Final_Player_Markers>> player_markers_raw,
          Map_Resources.Map map,
          uint Bene_Prediction
        )
        {
            Perspective_Generator = perspective_generator;
            Init = init;
            Players = players;
            Factions_Distribution_Raw = factions_distribution_raw;
            Player_Markers_Raw = player_markers_raw;
            Map = map;
            this.Bene_Prediction = Bene_Prediction;
        }

        public override string name => "Set-up";

        public override string moment { get; protected set; } = "";

        private I_Perspective_Generator Perspective_Generator { get; }

        private I_Setup_Initializers_And_Getters Init { get; }

        private IReadOnlySet<Faction> Factions_In_Play => Init.Factions_Distribution.Factions_In_Play;

        private Option<Either<Player_Markers_Manager, Final_Player_Markers>> Player_Markers_Raw { get; }

        private Forces Reserves => Init.Reserves;

        private I_Traitors_Initializer Traitors_Initializer => Init.Knowledge_Manager;

        private I_Spice_Manager Spice_Manager => Init.Knowledge_Manager;

        private I_Treachery_Cards_Manager Treachery_Cards_Manager => Init.Knowledge_Manager;

        private IReadOnlySet<Player> Players { get; }

        private Either<Factions_Distribution_Manager, Final_Factions_Distribution> Factions_Distribution_Raw { get; }

        private Map_Resources.Map Map { get; }

        private uint Bene_Prediction { get; set; }

        public override void Play_Out()
        {

            moment = "faction selection";
            {
                Factions_Distribution_Manager Factions_Distribution_Manager = Factions_Distribution_Raw.Left();

                //temporary assignment of the first faction from the free factions
                Players.ForEach(player => Factions_Distribution_Manager.Assign_Faction(player, Factions_Distribution_Manager.Free_Factions.First()));

                try
                {
                    Init.Init_Faction_Dependent_Objects();
                }
                catch (Faction_Selection_Ongoing)
                {
                    // keep the faction selection going
                }
            }

            moment = "player marker placement";
            {
                Player_Markers_Manager Player_Markers_Manager = Init.Player_Markers_Intermediary.Left();

                //temoporary assignment of the first player marker form the free markers
                Factions_In_Play.ForEach(faction => Player_Markers_Manager.Assign_Player_Marker(faction, Player_Markers_Manager.Free_Player_Markers.First()));

                try
                {
                    Init.Make_Player_Markers_Distribution_Final();
                }
                catch (Player_Marker_Selection_Ongoing)
                {
                    // keep the player marker selection ongoing
                }
            }
            Factions_In_Play.ForEach(faction => Console.WriteLine(Init.Factions_Distribution.Player_Of(faction).Id + " " + faction));
            moment = "Bene Gesserit prediction";
            Console.WriteLine("Bene Gesserit make a prediction: (/player2/Setup/3)");
            while (true)
            {

                string[] line = Console.ReadLine().Split("/"); //player1/Setup/3
                if (line[1] == Init.Factions_Distribution.Player_Of(Faction.Bene_Gesserit).Id && line[2] == "Setup")
                {
                    int response = Int32.Parse(line[3]);
                    if(response < 10 && response > 0)
                    {
                        Bene_Prediction = (uint)response;
                        break;
                    }
                }
            }

            moment = "traitor selection";
            
            IReadOnlyDictionary<Faction, IList<General>> traitors_dict = Generals_Manager.Random_Traitors(Factions_In_Play);
            Factions_In_Play.ForEach(faction => {
                if (faction == Faction.Harkonnen)
                {
                    Traitors_Initializer.Init_Traitors(faction, traitors_dict[faction].ToList(), []);
                    Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json");
                }
                else
                {
                    Traitors_Initializer.Init_Traitors(faction, traitors_dict[faction].ToList(), []);

                    Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json");

                    Console.WriteLine(faction.ToString() + " You have the traitors: ");
                    for(int i = 0; i  < 4; i++)
                    {
                        Console.WriteLine(traitors_dict[faction][i].Name);
                    }
                    Console.WriteLine("Choose one (/player1/Setup/traitor_name)");
                    bool end = true;
                    while (end)
                    {
                        string[] line = Console.ReadLine().Split("/");
                        if (line[1] == Init.Factions_Distribution.Player_Of(faction).Id && line[2] == "Setup")
                        {
                            for(int i = 0; i < traitors_dict[faction].ToList().Length(); i++)
                            {
                                if (traitors_dict[faction][i].Name == line[3])
                                {
                                    General traitor = traitors_dict[faction][i];
                                    traitors_dict[faction].RemoveAt(i);
                                    Traitors_Initializer.Init_Traitors(faction, [traitor], traitors_dict[faction].ToList());
                                    end = false;
                                    break;
                                }
                            }
                        }
                    }

                }
            });
            

            moment = "spice distribution";

            Factions_In_Play.ForEach(faction => {
                Spice_Manager.Add_Spice_To(faction, faction switch
                {
                    Faction.Atreides => 10,
                    Faction.Bene_Gesserit => 5,
                    Faction.Emperor => 10,
                    Faction.Fremen => 3,
                    Faction.Spacing_Guild => 5,
                    Faction.Harkonnen => 10,
                    _ => throw new Faction_Is_Not_Fully_Implemented(faction),
                });
            });

            moment = "initial faction forces placement";


            Factions_In_Play.ForEach(faction => ((Action)(faction switch
            {
                Faction.Atreides => () => Map.Arrakeen.Sections[0].Forces.Transfer_From(Faction.Atreides, Reserves, 10),
                Faction.Bene_Gesserit => () => Map.Polar_Sink.Sections[0].Forces.Transfer_From(Faction.Bene_Gesserit, Reserves, 1),
                Faction.Emperor => () => { }
                ,
                Faction.Fremen => () => { }
                ,
                Faction.Spacing_Guild => () => Map.Tuek_s_Sietch.Sections[0].Forces.Transfer_From(Faction.Spacing_Guild, Reserves, 5),
                Faction.Harkonnen => () => Map.Carthag.Sections[0].Forces.Transfer_From(Faction.Harkonnen, Reserves, 10),
                _ => throw new Faction_Is_Not_Fully_Implemented(faction),
            })).Invoke());

            if (Factions_In_Play.Contains(Faction.Fremen))
            {
                Forces To_Place_Now = new();
                To_Place_Now.Transfer_From(Faction.Fremen, Reserves, 10);

                int forces_distributed = 0;
                while (forces_distributed != 10)
                {
                     System.Console.WriteLine("Choose city for fremen and number of troops (/player4/Setup/14/10)");
                    string[] line = Console.ReadLine().Split("/");
                    if (line[1] == Init.Factions_Distribution.Player_Of(Faction.Fremen).Id && line[2] == "Setup")
                    {
                        Console.WriteLine("player is correct");
                        int sectionid = Int32.Parse(line[3]);
                        int troop_number = Int32.Parse(line[4]);
                        if (sectionid >= 0 && sectionid <= 84 && troop_number <= 10 - forces_distributed && troop_number > 0)
                        {
                            Console.WriteLine("section id and troop number is correct");
                            if (sectionid == 14 || sectionid == 15 || sectionid == 67 || (sectionid >= 73 && sectionid <= 75))
                            {
                                Map.Sections[sectionid].Forces.Transfer_From(Faction.Fremen, To_Place_Now, (uint)troop_number);
                                forces_distributed += troop_number;
                            }
                        }
                    }

                    // CELE 2 LINII DE MAI SUS ERAU DINAINTE SA MODIFIC EU, ERAU PT A VEDEA DACA MERGE JOCUL DIN CONSOLA
                    //while (ValidaterServerApi.canGet == false) //#
                    //{
                    //
                    //}
                    //ValidaterServerApi.canGet = false; //#
                    //string line = ValidaterServerApi.command; //#
                    //Console.WriteLine(line); //#

                    //UNDE AM ADAUGAT //#, e cod scris de mine
                    
                }
            }
            moment = "treachery card distribution";

            Factions_In_Play.ForEach(faction => {
                if (faction == Faction.Harkonnen)
                {
                    Treachery_Cards_Manager.Give_A_Treachery_Card(faction);
                }
                Treachery_Cards_Manager.Give_A_Treachery_Card(faction);
            });

            moment = "end of set-up";
            Factions_In_Play.ForEach(faction =>
            {
                Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json");
            });

            //round is set to 1 in 'Game' after this

        }
    }
}
