using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// storm sectors go from 0 to 17, the polar sink is considered to be the 18 th storm sector
// 0 is considered to be the starting sector in the rulebook (aka the one with 8 spice capacity in the cielago north region)
namespace dune_library.Map {
  internal class Map {
    public static ushort NUMBER_OF_SECTORS => 18;
    public static ushort To_Sector(ushort raw_section) {
      return (ushort)(raw_section % NUMBER_OF_SECTORS);
    }
    public static ushort To_Sector(int raw_section) {
      return To_Sector((ushort)raw_section);
    }
    public Map() {
      #region Initialisation of every region
      // regions have been initialised in increasing order by earliest sector crossed
      // if more regions start in the same sector, the one closer to the polar sink gets priority

      var Cielago_East = new Sand("Cielago East", 0, 2);

      var Harg_Pass = new Sand("Harg Pass", 1, 2);
      var False_Wall_South = new Rock("False Wall South", 1, 2);
      var South_Mesa = new Sand("South Mesa", 1, [null, 10, null]);

      var False_Wall_East = new Rock("False_Wall_East", 2, 5);
      var The_Minor_Erg = new Sand("The Minor Erg", 2, [null, null, null, 8]);
      var Pasty_Mesa = new Rock("Pasty_Mesa", 2, 4);
      var Tuek_s_Sietch = new Strongholds("Tuek's Sietch", 2);


      var Red_Chasm = new Sand("Red Chasm", 4, [8]);

      var Shield_Wall = new Rock("Shield_Wall", 5, 2);
      var Gara_Kulon = new Sand("Gara Kulon", 5, 1);

      var Imperial_Basin = new Sand("Imperial Basin", 6, 3);
      var Hole_In_The_Rock = new Sand("Hole In The Rock", 6, 1);
      var Rim_Wall_West = new Rock("Rim Wall West", 6, 1);
      var Basin = new Sand("Basin", 6, 1);
      var Sihaya_Ridge = new Sand("Sihaya Ridge", 6, [6]);
      var Old_Gap = new Sand("Old Gap", 6, [null, 6, null]);

      var Arrakeen = new Strongholds("Arrakeen", 7);

      var Arsunt = new Sand("Arsunt", 8, 2);
      var Carthag = new Strongholds("Carthag", 8);
      var Tsimpo = new Sand("Tsimpo", 8, 3);
      var Broken_Land = new Sand("Broken Land", 8, [null, 8]);

      var Hagga_Basin = new Sand("Hagga_Basin", 9, [null, 6]);
      var Plastic_Basin = new Rock("Plastic Basin", 9, 3);

      var Rock_Outcroppings = new Sand("Rock Outcroppings", 10, [null, 6]);

      var Wind_Pass = new Sand("Wind Pass", 11, 4);
      var Sietch_Tabr = new Strongholds("Sietch Tabr", 11);
      var Bight_Of_The_Cliff = new Sand("Bight Of The Cliff", 11, 2);

      var The_Great_Flat = new Sand("The Great Flat", 12, [10]);
      var Funeral_Plain = new Sand("Funeral Plain", 12, [6]);

      var The_Greater_Flat = new Sand("The Greater Flat", 13, 1);
      var False_Wall_West = new Rock("False Wall West", 13, 3);
      var Habbanya_Erg = new Sand("Habbanya Erg", 13, [8, null]);

      var Wind_Pass_North = new Sand("Wind Pass North", 14, [6, null]);
      var Habbanya_Ridge_Flat = new Sand("Habbanya Ridge Flat", 14, [null, 10]);
      var Habbanya_Sietch = new Strongholds("Habbanya Sietch", 14);

      var Cielago_West = new Sand("Cielago West", 15, 2);

      var Cielago_North = new Sand("Cielago North", 16, [null, null, 8]);
      var Cielago_Depression = new Sand("Cielago Depression", 16, 3);
      var Meridian = new Sand("Meridian", 16, 2);

      var Cielago_South = new Sand("Cielago South", 17, [12, null]);

      var Polar_Sink = new Polar_Sink("Polar Sink");

      #endregion

      #region Linking regions
      // order taken from the initialisation
      // neighbors are linked in approximate clockwise order starting from the north
      // (the direction of the polar sink)
      // commented neighbors are have already been linked, but are kept for maintenance reasons
      Link_All(Cielago_East, [Cielago_North,
                              False_Wall_South,
                              South_Mesa,
                              Cielago_South,
                              Cielago_Depression,]);
      Cielago_East.Block_Adding_Neighbors();
      Link_All(Harg_Pass, [Polar_Sink,
                           False_Wall_East,
                           The_Minor_Erg,
                           False_Wall_South,
                           Cielago_North,]);
      Harg_Pass.Block_Adding_Neighbors();
      Link_All(False_Wall_South, [/*Harg_Pass,*/
                                  The_Minor_Erg,
                                  Pasty_Mesa,
                                  Tuek_s_Sietch,
                                  South_Mesa,
                                  /*Cielago_East,*/
                                  Cielago_North,]);
      False_Wall_South.Block_Adding_Neighbors();
      Link_All(South_Mesa, [Tuek_s_Sietch,
                            Pasty_Mesa,
                            Red_Chasm,
                            /*Cielago_East,*/
                            /*False_Wall_South*/]);
      South_Mesa.Block_Adding_Neighbors();
      Link_All(False_Wall_East, [Polar_Sink,
                                 Imperial_Basin,
                                 Shield_Wall,
                                 The_Minor_Erg,
                                 /*Harg_Pass,*/]);
      False_Wall_East.Block_Adding_Neighbors();
      Link_All(The_Minor_Erg, [/*False_Wall_East,*/
                               Shield_Wall,
                               Pasty_Mesa,
                               /*False_Wall_South,*/
                               /*Harg_Pass,*/]);
      The_Minor_Erg.Block_Adding_Neighbors();
      Link_All(Pasty_Mesa, [/*The_Minor_Erg,*/
                            Shield_Wall,
                            Gara_Kulon,
                            Red_Chasm,
                            /*South_Mesa,*/
                            Tuek_s_Sietch,
                            /*False_Wall_South,*/]);
      Pasty_Mesa.Block_Adding_Neighbors();
      Link_All(Tuek_s_Sietch, [/*False_Wall_South,*/
                               /*Pasty_Mesa,*/
                               /*South_Mesa,*/]);
      Tuek_s_Sietch.Block_Adding_Neighbors();
      Link_All(Red_Chasm, [/*Pasty_Mesa,*/
                           /*South_Mesa,*/]);
      Red_Chasm.Block_Adding_Neighbors();
      Link_All(Shield_Wall, [/*False_Wall_East,*/
                             Imperial_Basin,
                             Hole_In_The_Rock,
                             Sihaya_Ridge,
                             Gara_Kulon,
                             /*Pasty_Mesa,*/
                             /*The_Minor_Erg,*/]);
      Shield_Wall.Block_Adding_Neighbors();
      Link_All(Gara_Kulon, [/*Pasty_Mesa,*/
                            /*Shield_Wall,*/
                            Sihaya_Ridge,]);
      Gara_Kulon.Block_Adding_Neighbors();
      Link_All(Imperial_Basin, [Polar_Sink,
                                Arsunt,
                                Carthag,
                                Tsimpo,
                                Old_Gap,
                                Arrakeen,
                                Rim_Wall_West,
                                Hole_In_The_Rock,
                                /*Shield_Wall,*/
                                /*False_Wall_East,*/]);
      Imperial_Basin.Block_Adding_Neighbors();
      Link_All(Hole_In_The_Rock, [/*Shield_Wall,*/
                                  /*Imperial_Basin,*/
                                  Rim_Wall_West,
                                  Basin,
                                  Sihaya_Ridge,]);
      Hole_In_The_Rock.Block_Adding_Neighbors();
      Link_All(Rim_Wall_West, [/*Imperial_Basin,*/
                               Arrakeen,
                               Old_Gap,
                               Basin,
                               /*Hole_In_The_Rock,*/]);
      Rim_Wall_West.Block_Adding_Neighbors();
      Link_All(Basin, [/*Hole_In_The_Rock,*/
                       /*Rim_Wall_West,*/
                       Old_Gap,
                       Sihaya_Ridge,]);
      Basin.Block_Adding_Neighbors();
      Link_All(Sihaya_Ridge, [/*Shield_Wall,*/
                              /*Hole_In_The_Rock,*/
                              /*Basin,*/
                              /*Gara_Kulon,*/]);
      Sihaya_Ridge.Block_Adding_Neighbors();
      Link_All(Old_Gap, [Arrakeen,
                         /*Imperial_Basin,*/
                         Tsimpo,
                         Broken_Land,
                         /*Basin,*/
                         /*Rim_Wall_West,*/]);
      Old_Gap.Block_Adding_Neighbors();
      Link_All(Arrakeen, [/*Imperial_Basin,*/
                          /*Old_Gap,*/
                          /*Rim_Wall_West,*/]);
      Arrakeen.Block_Adding_Neighbors();
      Link_All(Arsunt, [Polar_Sink,
                        Hagga_Basin,
                        Carthag,
                        /*Imperial_Basin,*/]);
      Arsunt.Block_Adding_Neighbors();
      Link_All(Carthag, [/*Arsunt,*/
                         Hagga_Basin,
                         Tsimpo,
                         /*Imperial_Basin,*/]);
      Carthag.Block_Adding_Neighbors();
      Link_All(Tsimpo, [/*Carthag,*/
                        Hagga_Basin,
                        Plastic_Basin,
                        Broken_Land,
                        /*Old_Gap,*/
                        /*Imperial_Basin,*/]);
      Tsimpo.Block_Adding_Neighbors();
      Link_All(Broken_Land, [/*Tsimpo,*/
                             Plastic_Basin,
                             Rock_Outcroppings,
                             /*Old_Gap,*/]);
      Broken_Land.Block_Adding_Neighbors();
      Link_All(Hagga_Basin, [Polar_Sink,
                             Wind_Pass,
                             Plastic_Basin,
                             /*Tsimpo,*/
                             /*Carthag,*/
                             /*Arsunt,*/]);
      Hagga_Basin.Block_Adding_Neighbors();
      Link_All(Plastic_Basin, [Wind_Pass,
                               The_Great_Flat,
                               Funeral_Plain,
                               Bight_Of_The_Cliff,
                               Sietch_Tabr,
                               Rock_Outcroppings,
                               /*Broken_Land,*/
                               /*Tsimpo,*/
                               /*Hagga_Basin,*/]);
      Plastic_Basin.Block_Adding_Neighbors();
      Link_All(Rock_Outcroppings, [/*Plastic_Basin,*/
                                   Sietch_Tabr,
                                   Bight_Of_The_Cliff,
                                   /*Broken_Land,*/]);
      Rock_Outcroppings.Block_Adding_Neighbors();
      Link_All(Wind_Pass, [Polar_Sink,
                           Wind_Pass_North,
                           Cielago_West,
                           False_Wall_West,
                           The_Greater_Flat,
                           The_Great_Flat,
                           /*Plastic_Basin,*/
                           /*Hagga_Basin,*/]);
      Wind_Pass.Block_Adding_Neighbors();
      Link_All(Sietch_Tabr, [/*Plastic_Basin,*/
                             Bight_Of_The_Cliff,
                             /*Rock_Outcroppings,*/]);
      Sietch_Tabr.Block_Adding_Neighbors();
      Link_All(Bight_Of_The_Cliff, [/*Plastic_Basin,*/
                                    Funeral_Plain,
                                    /*Rock_Outcroppings,*/
                                    /*Sietch_Tabr,*/]);
      Bight_Of_The_Cliff.Block_Adding_Neighbors();
      Link_All(The_Great_Flat, [/*Wind_Pass,*/
                                The_Greater_Flat,
                                Funeral_Plain,
                                /*Plastic_Basin,*/]);
      The_Greater_Flat.Block_Adding_Neighbors();
      Link_All(Funeral_Plain, [/*The_Great_Flat,*/
                               /*Bight_Of_The_Cliff,*/
                               /*Plastic_Basin,*/]);
      Funeral_Plain.Block_Adding_Neighbors();
      Link_All(The_Greater_Flat, [/*Wind_Pass,*/
                                  False_Wall_West,
                                  Habbanya_Erg,
                                  /*The_Great_Flat,*/]);
      The_Greater_Flat.Block_Adding_Neighbors();
      Link_All(False_Wall_West, [/*Wind_Pass,*/
                                 Cielago_West,
                                 Habbanya_Ridge_Flat,
                                 Habbanya_Erg,]);
      False_Wall_West.Block_Adding_Neighbors();
      Link_All(Habbanya_Erg, [/*False_Wall_West,*/
                              Habbanya_Ridge_Flat,
                              /*The_Greater_Flat,*/]);
      Habbanya_Erg.Block_Adding_Neighbors();
      Link_All(Wind_Pass_North, [Polar_Sink,
                                 Cielago_North,
                                 Cielago_West,
                                 /*Wind_Pass,*/]);
      Wind_Pass_North.Block_Adding_Neighbors();
      Link_All(Habbanya_Ridge_Flat, [/*False_Wall_West,*/
                                     Cielago_West,
                                     Meridian,
                                     /*Habbanya_Erg,*/
                                     Habbanya_Sietch,]);
      Habbanya_Ridge_Flat.Block_Adding_Neighbors();
      Link_All(Habbanya_Sietch, [/*Habbanya_Ridge_Flat,*/]);
      Habbanya_Sietch.Block_Adding_Neighbors();
      Link_All(Cielago_West, [/*Wind_Pass_North,*/
                              Cielago_North,
                              Cielago_Depression,
                              Meridian,
                              /*Habbanya_Ridge_Flat,*/
                              /*False_Wall_West,*/]);
      Cielago_West.Block_Adding_Neighbors();
      Link_All(Cielago_North, [Polar_Sink,
                               /*Harg_Pass,*/
                               /*False_Wall_South,*/
                               /*Cielago_East,*/
                               Cielago_Depression,
                               /*Cielago_West,*/
                               /*Wind_Pass_North,*/]);
      Cielago_North.Block_Adding_Neighbors();
      Link_All(Cielago_Depression, [/*Cielago_North,*/
                                    /*Cielago_East,*/
                                    Cielago_South,
                                    Meridian,
                                    /*Cielago_West,*/]);
      Cielago_Depression.Block_Adding_Neighbors();
      Link_All(Meridian, [/*Cielago_Depression,*/
                          Cielago_South,
                          /*Habbanya_Ridge_Flat,*/
                          /*Cielago_West,*/]);
      Meridian.Block_Adding_Neighbors();
      Link_All(Cielago_South, [/*Cielago_Depression,*/
                               /*Cielago_East,*/
                               /*Meridian,*/]);
      Cielago_South.Block_Adding_Neighbors();
      Link_All(Polar_Sink, [/*Harg_Pass,*/
                            /*False_Wall_East,*/
                            /*Imperial_Basin,*/
                            /*Arsunt,*/
                            /*Hagga_Basin,*/
                            /*Wind_Pass,*/
                            /*Wind_Pass_North,*/
                            /*Cielago_North,*/]);
      Polar_Sink.Block_Adding_Neighbors();

      #endregion

      #region Assigning regions to the dedicated containers

      sand_list = [
        Cielago_East,
        Harg_Pass,
        South_Mesa,
        The_Minor_Erg,
        Red_Chasm,
        Gara_Kulon,
        Imperial_Basin,
        Hole_In_The_Rock,
        Basin,
        Sihaya_Ridge,
        Old_Gap,
        Arsunt,
        Tsimpo,
        Broken_Land,
        Hagga_Basin,
        Rock_Outcroppings,
        Wind_Pass,
        Bight_Of_The_Cliff,
        The_Great_Flat,
        Funeral_Plain,
        The_Greater_Flat,
        Habbanya_Erg,
        Wind_Pass_North,
        Habbanya_Ridge_Flat,
        Cielago_West,
        Cielago_North,
        Cielago_Depression,
        Meridian,
        Cielago_South,
      ];
      rock_list = [
        False_Wall_South,
        False_Wall_East,
        Pasty_Mesa,
        Shield_Wall,
        Rim_Wall_West,
        Plastic_Basin,
        False_Wall_West,
      ];
      strongholds_list = [
        Tuek_s_Sietch,
        Arrakeen,
        Carthag,
        Sietch_Tabr,
        Habbanya_Sietch,
      ];
      polar_sink = Polar_Sink;

      #endregion

      #region Assigning sections for easier iteration in the storm phase
      // sections are taken from the polar sink to the meridian
      // commented sections would not be affected by the wind anyway
      // they're followed by eirther one of these comments:
      //     - rock section = rocks are not affected by wind
      //     - strongholds = strongholds are not affected by wind
      //     - family atomics = are not affected by wind before the family atomics card is played
      //                        but are affected by wind after  the family atomics card is played
      // none or all commented sections might get included later
      storm_affected_sections_by_sector = [ [
          // 0
          Cielago_North.Sections[2],
          Cielago_East.Sections[0],
          Cielago_Depression.Sections[2],
          Cielago_South.Sections[1],
        ], [
          // 1
          Harg_Pass.Sections[0],
          /*False_Wall_South.Sections[0],*/ // rock section
          Cielago_East.Sections[1],
          South_Mesa.Sections[0],
        ], [
          // 2
          /*False_Wall_East.Sections[0],*/ // rock section
          Harg_Pass.Sections[1],
          The_Minor_Erg.Sections[0],
          /*False_Wall_South.Sections[1],*/ // rock section
          /*Pasty_Mesa.Sections[0],*/ // rock section
          /*Tuek_s_Sietch.Sections[0],*/ // strongholds
          South_Mesa.Sections[1],
        ], [
          // 3
          /*False_Wall_East.Sections[1],*/ // rock section
          The_Minor_Erg.Sections[1],
          /*Pasty_Mesa.Sections[1],*/ // rock section
          South_Mesa.Sections[2],
        ], [
          // 4
          /*False_Wall_East.Sections[2],*/ // rock section
          The_Minor_Erg.Sections[2],
          /*Pasty_Mesa.Sections[2],*/ // rock section
          Red_Chasm.Sections[0],
        ], [
          // 5
          /*False_Wall_East.Sections[3],*/ // rock section
          /*Shield_Wall.Sections[0],*/ // rock section
          The_Minor_Erg.Sections[3],
          /*Pasty_Mesa.Sections[3],*/ // rock section
          Gara_Kulon.Sections[0],

        ], [
          // 6
          /*Imperial_Basin.Sections[0],*/ // family atomics
          /*False_Wall_East.Sections[4],*/ // rock section
          /*Shield_Wall.Sections[1],*/ // rock section
          Hole_In_The_Rock.Sections[0],
          /*Rim_Wall_West.Sections[0],*/ // rock section
          Basin.Sections[0],
          Sihaya_Ridge.Sections[0],
          Old_Gap.Sections[0],
        ], [
          // 7
          /*Imperial_Basin.Sections[1],*/ // family atomics
          /*Arrakeen.Sections[0],*/ // family atomics
          Old_Gap.Sections[1],
        ], [
          // 8
          Arsunt.Sections[0],
          /*Imperial_Basin.Sections[2],*/ // family atomics
          /*Carthag.Sections[0],*/ // family atomics
          Tsimpo.Sections[0],
          Broken_Land.Sections[0],
          Old_Gap.Sections[2],
        ], [
          // 9
          Arsunt.Sections[1],
          Hagga_Basin.Sections[0],
          Tsimpo.Sections[1],
          /*Plastic_Basin.Sections[0],*/ // rock section
          Broken_Land.Sections[1],

        ], [
          // 10
          Hagga_Basin.Sections[1],
          /*Plastic_Basin.Sections[1],*/ // rock section
          Tsimpo.Sections[2],
          Rock_Outcroppings.Sections[0],
        ], [
          // 11
          Wind_Pass.Sections[0],
          /*Plastic_Basin.Sections[2],*/ // rock section
          Bight_Of_The_Cliff.Sections[0],
          /*Sietch_Tabr.Sections[0],*/ // strongholds
          Rock_Outcroppings.Sections[1],
        ], [
          // 12
          Wind_Pass.Sections[1],
          The_Great_Flat.Sections[0],
          Funeral_Plain.Sections[0],
          Bight_Of_The_Cliff.Sections[1],
        ], [
          // 13
          Wind_Pass.Sections[2],
          The_Greater_Flat.Sections[0],
          /*False_Wall_West.Sections[0],*/ // rock section
          Habbanya_Erg.Sections[0],
        ], [
          // 14
          Wind_Pass_North.Sections[0],
          Wind_Pass.Sections[3],
          /*False_Wall_West.Sections[1],*/ // rock section
          Habbanya_Erg.Sections[1],
          Habbanya_Ridge_Flat.Sections[0],
          /*Habbanya_Sietch.Sections[0],*/ // strongholds
        ], [
          // 15
          Wind_Pass_North.Sections[1],
          Cielago_West.Sections[0],
          /*False_Wall_West.Sections[2],*/ // rock section
          Habbanya_Ridge_Flat.Sections[1],
        ], [
          // 16
          Cielago_North.Sections[0],
          Cielago_West.Sections[1],
          Cielago_Depression.Sections[0],
          Meridian.Sections[0],
        ], [
          // 17
          Cielago_North.Sections[1],
          Cielago_Depression.Sections[1],
          Meridian.Sections[1],
          Cielago_South.Sections[0],
        ],
      ];
      #endregion

      shield_wall_was_destroyed = false;

      #region Assigning relevant sections to the influenced_by_family_atomics list
      influenced_by_family_atomics = [
        Imperial_Basin.Sections[0],
        Imperial_Basin.Sections[1],
        Arrakeen.Sections[0],
        Imperial_Basin.Sections[2],
        Carthag.Sections[0],
      ];
      #endregion
    }

    private List<Sand> sand_list;
    private List<Rock> rock_list;
    private List<Strongholds> strongholds_list;
    private Polar_Sink polar_sink;

    private List<List<Section>> storm_affected_sections_by_sector;

    private bool shield_wall_was_destroyed;
    private List<Section> influenced_by_family_atomics;

    private static void Link(Region a, Region b) {
      a.Add_Neighbor(b);
      b.Add_Neighbor(a);
    }

    private static void Link_All(Region a, List<Region> bs) {
      bs.ForEach(b => Link(a, b));
    }

    public void destroy_shield_wall() {
      shield_wall_was_destroyed = true;
      // basically adds the imperial basin, arrakeen and carthag to the sections affected by the storm
      storm_affected_sections_by_sector[6].Add(influenced_by_family_atomics[0]);
      storm_affected_sections_by_sector[7].AddRange([influenced_by_family_atomics[1], influenced_by_family_atomics[2]]);
      storm_affected_sections_by_sector[8].AddRange([influenced_by_family_atomics[3], influenced_by_family_atomics[4]]);
    }

    public ushort Storm_Sector { get; private set; }

    public void move_storm_sector(ushort number_of_sectors) {
      // affect troops by storm
      Storm_Sector += 1;
      Enumerable.Range(Storm_Sector, Storm_Sector + number_of_sectors)
                .ToList().ForEach(pos => {
                  foreach (Section section in storm_affected_sections_by_sector[pos]) {
                    section.affect_by_storm();
                  }
                });
    }
  }
}
