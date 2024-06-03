using dune_library.Player_Resources;
using dune_library.Map_Resources;
using dune_library.Utils;
using dune_library.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using dune_library.Map_Resoures;
using LanguageExt;
using System.Reactive;
using dune_library.Player_Resources.Knowledge_Manager_Interfaces;

namespace dune_library.Phases
{
    public class SpiceCollectionPhase : Phase
    {
        public SpiceCollectionPhase(
        I_Setup_Initializers_And_Getters init,
        Map_Resources.Map map,
        Game game
    )
        {
            Init = init;
            Map = map;
            Perspective_Generator = game;
        }
        private I_Perspective_Generator Perspective_Generator { get; }
        public override string name => "Spice Collection";

        public override string moment { get; protected set; } = "";

        private I_Setup_Initializers_And_Getters Init { get; }

        private IReadOnlySet<Faction> Factions_In_Play => Init.Factions_Distribution.Factions_In_Play;

        private I_Spice_Manager Spice_Manager => Init.Knowledge_Manager;

        private Map_Resources.Map Map { get; }

        public override void Play_Out()
        {
            foreach (var section in Map.Sections_With_Spice)
            {
                if (section is With_Spice withSpice)
                {
                    if (withSpice.Spice_Avaliable > 0)
                    {
                        Factions_In_Play.ForEach(faction =>
                        {
                            var playerForces = withSpice.Forces.Of(faction);
                            if (playerForces > 0)
                            {
                                var spiceToCollect = playerForces * (IsKeyCity(withSpice) ? 3 : 2);
                                Spice_Manager.Add_Spice_To(faction,(uint)spiceToCollect);
                                withSpice.Take_Spice((uint)spiceToCollect);
                            }

                        });
                    }
                }
            }
            Init.Factions_Distribution.Factions_In_Play.ForEach(faction => Perspective_Generator.Generate_Perspective(Init.Factions_Distribution.Player_Of(faction)).SerializeToJson($"{Init.Factions_Distribution.Player_Of(faction).Id}.json"));
        }

        private bool IsKeyCity(With_Spice section)
        {
            return section.Origin_Territory.Name == "Arrakeen" || section.Origin_Territory.Name == "Carthag";
        }
    }
}