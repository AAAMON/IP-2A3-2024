using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// storm sectors go from 0 to 17, the polar sink is considered to be the 18 th storm sector
// 0 is considered to be the starting sector in the rulebook (aka the one with 8 spice capacity in the cielago north region)
namespace dune_library.Map_Resources {
  internal class Map {
    public static int NUMBER_OF_SECTORS => 18;

    public static int To_Sector(int raw_sector) {
      return raw_sector % NUMBER_OF_SECTORS;
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

      #region Linking Sections
      // sections are taken from the polar sink to the meridian
      // neighbors are linked in approximate clockwise order starting from the north
      // (the direction of the polar sink)
      // commented neighbors are have already been linked, but are kept for maintenance reasons
      // sections that are connected by an edge are omitted

      #region 0
      Link_All_And_Block(Cielago_North.Sections[2], [
        Polar_Sink.Sections[0],
        Harg_Pass.Sections[0],
        False_Wall_South.Sections[0],
        Cielago_East.Sections[0],
        Cielago_Depression.Sections[2],
        Cielago_North.Sections[1],
      ]);
      Link_All_And_Block(Cielago_East.Sections[0], [
        /*Cielago_North.Sections[2],*/
        Cielago_East.Sections[1],
        Cielago_South.Sections[1],
        Cielago_Depression.Sections[2],
      ]);
      Link_All_And_Block(Cielago_Depression.Sections[2], [
        /*Cielago_North.Sections[2],*/
        /*Cielago_East.Sections[0],*/
        Cielago_South.Sections[1],
        Cielago_Depression.Sections[1],
      ]);
      Link_All_And_Block(Cielago_South.Sections[1], [
        /*Cielago_Depression.Sections[2],*/
        /*Cielago_East.Sections[0],*/
        Cielago_South.Sections[0],
      ]);
      #endregion

      #region 1
      Link_All_And_Block(Harg_Pass.Sections[0], [
        Polar_Sink.Sections[0],
        False_Wall_East.Sections[0],
        Harg_Pass.Sections[1],
        False_Wall_South.Sections[0],
        /*Cielago_North.Sections[2],*/
      ]);
      Link_All_And_Block(False_Wall_South.Sections[0], [
        /*Harg_Pass.Sections[0],*/
        False_Wall_South.Sections[1],
        South_Mesa.Sections[0],
        Cielago_East.Sections[1],
        /*Cielago_North.Sections[2],*/
      ]);
      Link_All_And_Block(Cielago_East.Sections[1], [
        /*False_Wall_South.Sections[0],*/
        South_Mesa.Sections[0],
        /*Cielago_East.Sections[0],*/
      ]);
      Link_All_And_Block(South_Mesa.Sections[0], [
        /*False_Wall_South.Sections[0],*/
        South_Mesa.Sections[1],
        /*Cielago_East.Sections[1],*/
      ]);
      #endregion

      #region 2
      Link_All_And_Block(False_Wall_East.Sections[0], [
        Polar_Sink.Sections[0],
        False_Wall_East.Sections[1],
        The_Minor_Erg.Sections[0],
        Harg_Pass.Sections[1],
        /*Harg_Pass.Sections[0],*/
      ]);
      Link_All_And_Block(Harg_Pass.Sections[1], [
        False_Wall_East.Sections[0],
        The_Minor_Erg.Sections[0],
        False_Wall_South.Sections[1],
        /*Harg_Pass.Sections[0],*/
      ]);
      Link_All_And_Block(The_Minor_Erg.Sections[0], [
        /*False_Wall_East.Sections[0],*/
        The_Minor_Erg.Sections[1],
        Pasty_Mesa.Sections[0],
        False_Wall_South.Sections[1],
        /*Harg_Pass.Sections[1],*/
      ]);
      Link_All_And_Block(False_Wall_South.Sections[1], [
        /*Harg_Pass.Sections[1],*/
        /*The_Minor_Erg.Sections[0],*/
        Pasty_Mesa.Sections[0],
        Tuek_s_Sietch.Sections[0],
        South_Mesa.Sections[1],
        /*False_Wall_South.Sections[0],*/
      ]);
      Link_All_And_Block(Pasty_Mesa.Sections[0], [
        /*The_Minor_Erg.Sections[0],*/
        Pasty_Mesa.Sections[1],
        South_Mesa.Sections[1],
        Tuek_s_Sietch.Sections[0],
        /*False_Wall_South.Sections[1],*/
      ]);
      Link_All_And_Block(Tuek_s_Sietch.Sections[0], [
        /*Pasty_Mesa.Sections[0],*/
        South_Mesa.Sections[1],
        /*False_Wall_South.Sections[1],*/
      ]);
      Link_All_And_Block(South_Mesa.Sections[1], [
        /*Tuek_s_Sietch.Sections[0],*/
        /*Pasty_Mesa.Sections[0],*/
        South_Mesa.Sections[2],
        /*South_Mesa.Sections[0],*/
        /*False_Wall_South.Sections[1],*/
      ]);
      #endregion

      #region 3
      Link_All_And_Block(False_Wall_East.Sections[1], [
        Polar_Sink.Sections[0],
        False_Wall_East.Sections[2],
        The_Minor_Erg.Sections[1],
        /*False_Wall_East.Sections[0],*/
      ]);
      Link_All_And_Block(The_Minor_Erg.Sections[1], [
        /*False_Wall_East.Sections[1],*/
        The_Minor_Erg.Sections[2],
        Pasty_Mesa.Sections[1],
        /*The_Minor_Erg.Sections[0],*/
      ]);
      Link_All_And_Block(Pasty_Mesa.Sections[1], [
        /*The_Minor_Erg.Sections[1],*/
        Pasty_Mesa.Sections[2],
        South_Mesa.Sections[2],
        /*Pasty_Mesa.Sections[0],*/
      ]);
      Link_All_And_Block(South_Mesa.Sections[2], [
        /*Pasty_Mesa.Sections[1],*/
        Red_Chasm.Sections[0],
        /*South_Mesa.Sections[2],*/
      ]);
      #endregion

      #region 4
      Link_All_And_Block(False_Wall_East.Sections[2], [
        Polar_Sink.Sections[0],
        False_Wall_East.Sections[3],
        The_Minor_Erg.Sections[2],
        /*False_Wall_East.Sections[1],*/
      ]);
      Link_All_And_Block(The_Minor_Erg.Sections[2], [
        /*False_Wall_East.Sections[2],*/
        The_Minor_Erg.Sections[3],
        Pasty_Mesa.Sections[2],
        /*The_Minor_Erg.Sections[1],*/
      ]);
      Link_All_And_Block(Pasty_Mesa.Sections[2], [
        /*The_Minor_Erg.Sections[2],*/
        Pasty_Mesa.Sections[3],
        Red_Chasm.Sections[0],
        /*Pasty_Mesa.Sections[1],*/
      ]);
      Link_All_And_Block(Red_Chasm.Sections[0], [
        /*Pasty_Mesa.Sections[2],*/
        /*South_Mesa.Sections[2],*/
      ]);
      #endregion

      #region 5
      Link_All_And_Block(False_Wall_East.Sections[3], [
        Polar_Sink.Sections[0],
        False_Wall_East.Sections[4],
        Shield_Wall.Sections[0],
        The_Minor_Erg.Sections[3],
        /*False_Wall_East.Sections[2],*/
      ]);
      Link_All_And_Block(Shield_Wall.Sections[0], [
        /*False_Wall_East.Sections[3],*/
        Shield_Wall.Sections[1],
        Gara_Kulon.Sections[0],
        Pasty_Mesa.Sections[3],
        The_Minor_Erg.Sections[3],
      ]);
      Link_All_And_Block(The_Minor_Erg.Sections[3], [
        /*False_Wall_East.Sections[3],*/
        /*Shield_Wall.Sections[0],*/
        Pasty_Mesa.Sections[3],
        /*The_Minor_Erg.Sections[2],*/
      ]);
      Link_All_And_Block(Pasty_Mesa.Sections[3], [
        /*The_Minor_Erg.Sections[3],*/
        /*Shield_Wall.Sections[0],*/
        Gara_Kulon.Sections[0],
        Pasty_Mesa.Sections[2],
      ]);
      Link_All_And_Block(Gara_Kulon.Sections[0], [
        /*Pasty_Mesa.Sections[3],*/
        /*Shield_Wall.Sections[0],*/
        Sihaya_Ridge.Sections[0],
      ]);
      #endregion

      #region 6
      Link_All_And_Block(Imperial_Basin.Sections[0], [
        Polar_Sink.Sections[0],
        Imperial_Basin.Sections[1],
        Rim_Wall_West.Sections[0],
        Hole_In_The_Rock.Sections[0],
        Shield_Wall.Sections[1],
        False_Wall_East.Sections[4],
      ]);
      Link_All_And_Block(False_Wall_East.Sections[4], [
        Polar_Sink.Sections[0],
        /*Imperial_Basin.Sections[0],*/
        Shield_Wall.Sections[1],
        /*False_Wall_East.Sections[3],*/
      ]);
      Link_All_And_Block(Shield_Wall.Sections[1], [
        /*False_Wall_East.Sections[4],*/
        /*Imperial_Basin.Sections[0],*/
        Hole_In_The_Rock.Sections[0],
        Sihaya_Ridge.Sections[0],
        /*Shield_Wall.Sections[0],*/
      ]);
      Link_All_And_Block(Hole_In_The_Rock.Sections[0], [
        /*Shield_Wall.Sections[1],*/
        /*Imperial_Basin.Sections[0],*/
        Rim_Wall_West.Sections[0],
        Basin.Sections[0],
        Sihaya_Ridge.Sections[0],
      ]);
      Link_All_And_Block(Rim_Wall_West.Sections[0], [
        /*Imperial_Basin.Sections[0],*/
        Imperial_Basin.Sections[1],
        Arrakeen.Sections[0],
        Old_Gap.Sections[0],
        Basin.Sections[0],
        /*Hole_In_The_Rock.Sections[0],*/
      ]);
      Link_All_And_Block(Basin.Sections[0], [
        /*Hole_In_The_Rock.Sections[0],*/
        /*Rim_Wall_West.Sections[0],*/
        Old_Gap.Sections[0],
        Sihaya_Ridge.Sections[0],
      ]);
      Link_All_And_Block(Sihaya_Ridge.Sections[0], [
        /*Shield_Wall.Sections[1],*/
        /*Hole_In_The_Rock.Sections[0],*/
        /*Basin.Sections[0],*/
        /*Gara_Kulon.Sections[0],*/
      ]);
      Link_All_And_Block(Old_Gap.Sections[0], [
        /*Rim_Wall_West.Sections[0],*/
        Old_Gap.Sections[1],
        /*Basin.Sections[0],*/
      ]);
      #endregion

      #region 7
      Link_All_And_Block(Imperial_Basin.Sections[1], [
        Polar_Sink.Sections[0],
        Imperial_Basin.Sections[2],
        Old_Gap.Sections[1],
        Arrakeen.Sections[0],
        /*Rim_Wall_West.Sections[0],*/
        /*Imperial_Basin.Sections[0],*/
      ]);
      Link_All_And_Block(Arrakeen.Sections[0], [
        Imperial_Basin.Sections[1],
        Old_Gap.Sections[1],
        /*Rim_Wall_West.Sections[0],*/
      ]);
      Link_All_And_Block(Old_Gap.Sections[1], [
        /*Arrakeen.Sections[0],*/
        /*Imperial_Basin.Sections[1],*/
        Old_Gap.Sections[2],
        /*Old_Gap.Sections[0],*/
      ]);
      #endregion

      #region 8
      Link_All_And_Block(Arsunt.Sections[0], [
        Polar_Sink.Sections[0],
        Arsunt.Sections[1],
        Hagga_Basin.Sections[0],
        Carthag.Sections[0],
        Imperial_Basin.Sections[2],
      ]);
      Link_All_And_Block(Imperial_Basin.Sections[2], [
        /*Arsunt.Sections[0],*/
        Carthag.Sections[0],
        Tsimpo.Sections[0],
        /*Imperial_Basin.Sections[1],*/
      ]);
      Link_All_And_Block(Carthag.Sections[0], [
        /*Arsunt.Sections[0],*/
        Hagga_Basin.Sections[0],
        Tsimpo.Sections[1],
        Tsimpo.Sections[0],
        /*Imperial_Basin.Sections[2],*/
      ]);
      Link_All_And_Block(Tsimpo.Sections[0], [
        /*Carthag.Sections[0],*/
        Tsimpo.Sections[1],
        Broken_Land.Sections[0],
        Old_Gap.Sections[2],
        /*Imperial_Basin.Sections[2],*/
      ]);
      Link_All_And_Block(Broken_Land.Sections[0], [
        /*Tsimpo.Sections[0],*/
        Broken_Land.Sections[1],
        /*Old_Gap.Sections[2],*/
      ]);
      Link_All_And_Block(Old_Gap.Sections[2], [
        /*Tsimpo.Sections[0],*/
        /*Broken_Land.Sections[0],*/
        /*Old_Gap.Sections[1],*/
      ]);
      #endregion

      #region 9
      Link_All_And_Block(Arsunt.Sections[1], [
        Polar_Sink.Sections[0],
        Hagga_Basin.Sections[1],
        Hagga_Basin.Sections[0],
        /*Arsunt.Sections[0],*/
      ]);
      Link_All_And_Block(Hagga_Basin.Sections[0], [
        /*Arsunt.Sections[1],*/
        Hagga_Basin.Sections[1],
        Tsimpo.Sections[1],
        /*Carthag.Sections[0],*/
        /*Arsunt.Sections[0],*/
      ]);
      Link_All_And_Block(Tsimpo.Sections[1], [
        /*Hagga_Basin.Sections[0],*/
        Tsimpo.Sections[2],
        Plastic_Basin.Sections[0],
        Broken_Land.Sections[1],
        /*Tsimpo.Sections[0],*/
        /*Carthag.Sections[0],*/
      ]);
      Link_All_And_Block(Plastic_Basin.Sections[0], [
        /*Tsimpo.Sections[1],*/
        Plastic_Basin.Sections[1],
        Broken_Land.Sections[1],
      ]);
      Link_All_And_Block(Broken_Land.Sections[1], [
        /*Plastic_Basin.Sections[0],*/
        Rock_Outcroppings.Sections[0],
        /*Broken_Land.Sections[0],*/
        /*Tsimpo.Sections[1],*/
      ]);
      #endregion

      #region 10
      Link_All_And_Block(Hagga_Basin.Sections[1], [
        Polar_Sink.Sections[0],
        Wind_Pass.Sections[0],
        Plastic_Basin.Sections[2],
        Plastic_Basin.Sections[1],
        Tsimpo.Sections[2],
        /*Hagga_Basin.Sections[0],*/
        /*Arsunt.Sections[1],*/
      ]);
      Link_All_And_Block(Plastic_Basin.Sections[1], [
        /*Hagga_Basin.Sections[1],*/
        Plastic_Basin.Sections[2],
        Rock_Outcroppings.Sections[0],
        /*Plastic_Basin.Sections[0],*/
        Tsimpo.Sections[2],
      ]);
      Link_All_And_Block(Tsimpo.Sections[2], [
        /*Hagga_Basin.Sections[1],*/
        /*Plastic_Basin.Sections[1],*/
        /*Tsimpo.Sections[1],*/
      ]);
      Link_All_And_Block(Rock_Outcroppings.Sections[0], [
        /*Plastic_Basin.Sections[1],*/
        Rock_Outcroppings.Sections[1],
        /*Broken_Land.Sections[1],*/
      ]);
      #endregion

      #region 11
      Link_All_And_Block(Wind_Pass.Sections[0], [
        Polar_Sink.Sections[0],
        Wind_Pass.Sections[1],
        Plastic_Basin.Sections[2],
        /*Hagga_Basin.Sections[1],*/
      ]);
      Link_All_And_Block(Plastic_Basin.Sections[2], [
        /*Wind_Pass.Sections[0],*/
        The_Great_Flat.Sections[0],
        Funeral_Plain.Sections[0],
        Bight_Of_The_Cliff.Sections[0],
        Sietch_Tabr.Sections[0],
        Rock_Outcroppings.Sections[1],
        /*Plastic_Basin.Sections[1],*/
        /*Hagga_Basin.Sections[1],*/
      ]);
      Link_All_And_Block(Bight_Of_The_Cliff.Sections[0], [
        /*Plastic_Basin.Sections[2],*/
        Bight_Of_The_Cliff.Sections[1],
        Rock_Outcroppings.Sections[1],
        Sietch_Tabr.Sections[0],
      ]);
      Link_All_And_Block(Sietch_Tabr.Sections[0], [
        /*Plastic_Basin.Sections[2],*/
        /*Bight_Of_The_Cliff.Sections[0],*/
        Rock_Outcroppings.Sections[1],
      ]);
      Link_All_And_Block(Rock_Outcroppings.Sections[1], [
        /*Plastic_Basin.Sections[2],*/
        /*Sietch_Tabr.Sections[0],*/
        /*Bight_Of_The_Cliff.Sections[0],*/
        /*Rock_Outcroppings.Sections[0],*/
      ]);
      #endregion

      #region 12
      Link_All_And_Block(Wind_Pass.Sections[1], [
        Polar_Sink.Sections[0],
        Wind_Pass.Sections[2],
        The_Great_Flat.Sections[0],
        /*Wind_Pass.Sections[0],*/
      ]);
      Link_All_And_Block(The_Great_Flat.Sections[0], [
        /*Wind_Pass.Sections[1],*/
        The_Greater_Flat.Sections[0],
        Funeral_Plain.Sections[0],
        /*Plastic_Basin.Sections[2],*/
      ]);
      Link_All_And_Block(Funeral_Plain.Sections[0], [
        /*The_Great_Flat.Sections[0],*/
        Bight_Of_The_Cliff.Sections[1],
        /*Plastic_Basin.Sections[2],*/
      ]);
      Link_All_And_Block(Bight_Of_The_Cliff.Sections[1], [
        /*Funeral_Plain.Sections[0],*/
        /*Bight_Of_The_Cliff.Sections[0],*/
      ]);
      #endregion

      #region 13
      Link_All_And_Block(Wind_Pass.Sections[2], [
        Polar_Sink.Sections[0],
        Wind_Pass_North.Sections[0],
        Wind_Pass.Sections[3],
        False_Wall_West.Sections[0],
        The_Greater_Flat.Sections[0],
        /*Wind_Pass.Sections[1],*/
      ]);
      Link_All_And_Block(The_Greater_Flat.Sections[0], [
        /*Wind_Pass.Sections[2],*/
        False_Wall_West.Sections[0],
        Habbanya_Erg.Sections[0],
        The_Great_Flat.Sections[0],
      ]);
      Link_All_And_Block(False_Wall_West.Sections[0], [
        /*Wind_Pass.Sections[2],*/
        False_Wall_West.Sections[1],
        /*The_Greater_Flat.Sections[0],*/
      ]);
      Link_All_And_Block(Habbanya_Erg.Sections[0], [
        /*The_Greater_Flat.Sections[0],*/
        Habbanya_Erg.Sections[1],
        Habbanya_Ridge_Flat.Sections[0],
      ]);
      #endregion

      #region 14
      Link_All_And_Block(Wind_Pass_North.Sections[0], [
        Polar_Sink.Sections[0],
        Wind_Pass_North.Sections[1],
        Wind_Pass.Sections[3],
        /*Wind_Pass.Sections[2],*/
      ]);
      Link_All_And_Block(Wind_Pass.Sections[3], [
        /*Wind_Pass_North.Sections[0],*/
        Cielago_West.Sections[0],
        False_Wall_West.Sections[1],
        /*Wind_Pass.Sections[2],*/
      ]);
      Link_All_And_Block(False_Wall_West.Sections[1], [
        /*Wind_Pass.Sections[3],*/
        False_Wall_West.Sections[2],
        Habbanya_Ridge_Flat.Sections[0],
        Habbanya_Erg.Sections[1],
        /*False_Wall_West.Sections[0],*/
      ]);
      Link_All_And_Block(Habbanya_Erg.Sections[1], [
        /*False_Wall_West.Sections[1],*/
        Habbanya_Ridge_Flat.Sections[0],
        /*Habbanya_Erg.Sections[0],*/
      ]);
      Link_All_And_Block(Habbanya_Ridge_Flat.Sections[0], [
        /*Habbanya_Erg.Sections[1],*/
        /*False_Wall_West.Sections[1],*/
        Habbanya_Ridge_Flat.Sections[1],
        Habbanya_Sietch.Sections[0],
        /*Habbanya_Erg.Sections[0],*/
      ]);
      Link_All_And_Block(Habbanya_Sietch.Sections[0], [
        /*Habbanya_Ridge_Flat.Sections[0],*/
        Habbanya_Ridge_Flat.Sections[1],
      ]);
      #endregion

      #region 15
      Link_All_And_Block(Wind_Pass_North.Sections[1], [
        Polar_Sink.Sections[0],
        Cielago_North.Sections[0],
        Cielago_West.Sections[0],
        /*Wind_Pass_North.Sections[0],*/
      ]);
      Link_All_And_Block(Cielago_West.Sections[0], [
        /*Wind_Pass_North.Sections[1],*/
        Cielago_North.Sections[0],
        Cielago_West.Sections[1],
        Habbanya_Ridge_Flat.Sections[1],
        False_Wall_West.Sections[2],
      ]);
      Link_All_And_Block(False_Wall_West.Sections[2], [
        /*Cielago_West.Sections[0],*/
        Habbanya_Ridge_Flat.Sections[1],
        /*False_Wall_West.Sections[1],*/
      ]);
      Link_All_And_Block(Habbanya_Ridge_Flat.Sections[1], [
        /*False_Wall_West.Sections[2],*/
        /*Cielago_West.Sections[0],*/
        Meridian.Sections[0],
        /*Habbanya_Ridge_Flat.Sections[0],*/
        /*Habbanya_Sietch.Sections[0],*/
      ]);
      #endregion

      #region 16
      Link_All_And_Block(Cielago_North.Sections[0], [
        Polar_Sink.Sections[0],
        Cielago_North.Sections[1],
        Cielago_Depression.Sections[0],
        Cielago_West.Sections[1],
        /*Cielago_West.Sections[0],*/
        /*Wind_Pass_North.Sections[1],*/
      ]);
      Link_All_And_Block(Cielago_West.Sections[1], [
        /*Cielago_North.Sections[0],*/
        Cielago_Depression.Sections[0],
        Meridian.Sections[0],
        /*Cielago_West.Sections[0],*/
      ]);
      Link_All_And_Block(Cielago_Depression.Sections[0], [
        /*Cielago_North.Sections[0],*/
        Cielago_Depression.Sections[1],
        Meridian.Sections[0],
        /*Cielago_West.Sections[1],*/
      ]);
      Link_All_And_Block(Meridian.Sections[0], [
        /*Cielago_Depression.Sections[0],*/
        Meridian.Sections[1],
        /*Habbanya_Ridge_Flat.Sections[1],*/
        /*Cielago_West.Sections[1],*/
      ]);
      #endregion

      #region 17
      Link_All_And_Block(Cielago_North.Sections[1], [
        Polar_Sink.Sections[0],
        /*Cielago_North.Sections[2],*/
        Cielago_Depression.Sections[1],
        /*Cielago_North.Sections[0],*/
      ]);
      Link_All_And_Block(Cielago_Depression.Sections[1], [
        /*Cielago_North.Sections[1],*/
        /*Cielago_Depression.Sections[2],*/
        Cielago_South.Sections[0],
        Meridian.Sections[1],
        /*Cielago_Depression.Sections[0],*/
      ]);
      Link_All_And_Block(Meridian.Sections[1], [
        /*Cielago_Depression.Sections[1],*/
        Cielago_South.Sections[0],
        /*Meridian.Sections[0],*/
      ]);
      Link_All_And_Block(Cielago_South.Sections[0], [
        /*Cielago_Depression.Sections[1],*/
        /*Cielago_South.Sections[1],*/
        /*Meridian.Sections[1],*/
      ]);
      #endregion

      #region 18
      Link_All_And_Block(Polar_Sink.Sections[0], [
        /*Cielago_North.Sections[2],*/
        /*Harg_Pass.Sections[0],*/
        /*False_Wall_East.Sections[0],*/
        /*False_Wall_East.Sections[1],*/
        /*False_Wall_East.Sections[2],*/
        /*False_Wall_East.Sections[3],*/
        /*False_Wall_East.Sections[4],*/
        /*Imperial_Basin.Sections[0],*/
        /*Imperial_Basin.Sections[1],*/
        /*Arsunt.Sections[0],*/
        /*Arsunt.Sections[1],*/
        /*Hagga_Basin.Sections[0],*/
        /*Wind_Pass.Sections[0],*/
        /*Wind_Pass.Sections[1],*/
        /*Wind_Pass.Sections[2],*/
        /*Wind_Pass_North.Sections[0],*/
        /*Wind_Pass_North.Sections[1],*/
        /*Cielago_North.Sections[0],*/
        /*Cielago_North.Sections[1],*/
      ]);
      #endregion

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
      Storm_Affected_Sections_By_Sector = [ [
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

    public List<List<Section>> Storm_Affected_Sections_By_Sector { get; }

    private bool shield_wall_was_destroyed = false;
    private List<Section> influenced_by_family_atomics;

    private static void Link(Section a, Section b) {
      a.Add_Neighbor(b);
      b.Add_Neighbor(a);
    }

    private static void Link_All(Section a, IEnumerable<Section> bs) {
      foreach (Section b in bs) { Link(a, b); }
    }

    private static void Link_All_And_Block(Section a, IEnumerable<Section> bs) {
      Link_All(a, bs);
      a.Block_Adding_Neighbors();
    }

    public void Destroy_Shield_Wall() {
      shield_wall_was_destroyed = true;
      // basically adds the imperial basin, arrakeen and carthag to the sections affected by the storm
      influenced_by_family_atomics.ForEach(section => {
        Storm_Affected_Sections_By_Sector[section.Origin_Sector].Add(section);
      });
    }
  }
}
