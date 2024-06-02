using dune_library.Map_Resources;
using dune_library.Map_Resoures;
using dune_library.Player_Resources;
using dune_library.Player_Resources.Knowledge_Manager_Interfaces;
using dune_library.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Phases
{
    public class CharityPhase : Phase
    {
        private I_Spice_Manager Spice_Manager => Init.Knowledge_Manager;
        private IReadOnlySet<Faction> Factions_In_Play => Init.Factions_Distribution.Factions_In_Play;
        private I_Setup_Initializers_And_Getters Init { get; }

        private I_Perspective_Generator Perspective_Generator { get; }

        private IReadOnlySet<Player> Players { get; }

        public CharityPhase(I_Setup_Initializers_And_Getters init, Game game)
        {
            this.Init = init;
            Perspective_Generator = game;
            Players = game.Players;
            Input_Provider = game.Input_Provider;
        }
        public I_Input_Provider Input_Provider { get; set; }
        public override string name => "Charity Phase";

        public override string moment { get; protected set; } = "charity";

        public override void Play_Out()
        {
            moment = "before sending spice";
            //string Get_Card = Wait_Until_Something.AwaitInput(3000, Input_Provider).Result;
            //Console.WriteLine(Get_Card);

            Factions_In_Play.ForEach(faction =>
            {
                if(faction == Faction.Bene_Gesserit)
                {
                    Spice_Manager.Add_Spice_To(faction, 2);
                    Console.WriteLine("Added spice to Bene");
                }
                else if (Spice_Manager.getSpice(faction) < 2)
                {
                    Spice_Manager.Add_Spice_To(faction, 2 - Spice_Manager.getSpice(faction));
                }
            });

            moment = "spice was sent";
            //Get_Card = Wait_Until_Something.AwaitInput(3000, Input_Provider).Result;
            //Console.WriteLine(Get_Card);

            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));

        }
    }
}