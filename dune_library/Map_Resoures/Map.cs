using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using dune_library.Map_Resoures;
using System.Text.Json.Serialization;
using static System.Collections.Specialized.BitVector32;

// storm sectors go from 0 to 17, the polar sink is considered to be the 18 th storm sector
// 0 is considered to be the starting sector in the rulebook (aka the one with 8 spice capacity in the cielago north region)
namespace dune_library.Map_Resources {
  static public class Lookup {
    static public Territory To_Territory(this int id) {
      if (id > Map.Territories.Count) {
        throw new ArgumentException("no territory is mapped to this id (max: " + Map.Territories.Count + ", id: " + id + ")");
      }
      return Map.Territories[id];
    }
    static public Section To_Section(this int id) {
      if (id > Map.Sections.Count) {
        throw new ArgumentException("no section is mapped to this id (max: " + Map.Sections.Count + ", id: " + id + ")");
      }
      return Map.Sections[id];
    }

    static public With_Spice To_Section_With_Spice(this int id) {
      if (id > Map.Sections_With_Spice.Count) {
        throw new ArgumentException("no section with spice is mapped to this id (max: " + Map.Sections_With_Spice.Count + ", id: " + id + ")");
      }
      return Map.Sections_With_Spice[id];
    }
  }

  public class Map {
    static Map() {
      #region Territories Initialization

      Territories = [
        Cielago_North,
        Cielago_Depression,
        Meridian,

        Cielago_South,

        Cielago_East,

        Harg_Pass,
        False_Wall_South,
        South_Mesa,

        False_Wall_East,
        The_Minor_Erg,
        Pasty_Mesa,
        Tuek_s_Sietch,


        Red_Chasm,

        Shield_Wall,
        Gara_Kulon,

        Imperial_Basin,
        Hole_In_The_Rock,
        Rim_Wall_West,
        Basin,
        Sihaya_Ridge,
        Old_Gap,

        Arrakeen,

        Arsunt,
        Carthag,
        Tsimpo,
        Broken_Land,

        Hagga_Basin,
        Plastic_Basin,

        Rock_Outcroppings,

        Wind_Pass,
        Sietch_Tabr,
        Bight_Of_The_Cliff,

        The_Great_Flat,
        Funeral_Plain,

        The_Greater_Flat,
        False_Wall_West,
        Habbanya_Erg,

        Wind_Pass_North,
        Habbanya_Ridge_Flat,
        Habbanya_Sietch,

        Cielago_West,

        Polar_Sink,
      ];

      #endregion

      Sections = Territories.SelectMany(l => l.Sections).ToList();

      Sections_With_Spice = Sections.OfType<With_Spice>().ToList();

      #region Linking Sections

      // sections are taken from the polar sink to the meridian
      // neighbors are linked in approximate clockwise order starting from the north
      // (the direction of the polar sink)
      // commented neighbors are have already been linked, but are kept for maintenance reasons
      // sections that are connected by an edge are omitted

      #region 0
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

      #region 1
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

      #region 2
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

      #region 3
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

      #region 4
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

      #region 5
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

      #region 6
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

      #region 7
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

      #region 8
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

      #region 9
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

      #region 10
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

      #region 11
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

      #region 12
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

      #region 13
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

      #region 14
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

      #region 15
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

      #region 16
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

      #region 17
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

      #region Storm Affectable Initialization

      // sections are taken from the polar sink to the meridian
      // commented sections would not be affected by the wind anyway
      // they're followed by either one of these comments:
      //     - rock section = rocks are not affected by wind
      //     - strongholds = strongholds are not affected by wind
      //     - family atomics = are not affected by wind before the family atomics card is played
      //                        but are affected by wind after  the family atomics card is played
      // none or all commented sections might get included later

      Storm_Affectable = [ [
          // 0
          Cielago_North.Sections[0],
          Cielago_West.Sections[1],
          Cielago_Depression.Sections[0],
          Meridian.Sections[0],
        ], [
          // 1
          Cielago_North.Sections[1],
          Cielago_Depression.Sections[1],
          Meridian.Sections[1],
          Cielago_South.Sections[0],
        ], [
          // 2
          Cielago_North.Sections[2],
          Cielago_East.Sections[0],
          Cielago_Depression.Sections[2],
          Cielago_South.Sections[1],
        ], [
          // 3
          Harg_Pass.Sections[0],
          /*False_Wall_South.Sections[0],*/ // rock section
          Cielago_East.Sections[1],
          South_Mesa.Sections[0],
        ], [
          // 4
          /*False_Wall_East.Sections[0],*/ // rock section
          Harg_Pass.Sections[1],
          The_Minor_Erg.Sections[0],
          /*False_Wall_South.Sections[1],*/ // rock section
          /*Pasty_Mesa.Sections[0],*/ // rock section
          /*Tuek_s_Sietch.Sections[0],*/ // strongholds
          South_Mesa.Sections[1],
        ], [
          // 5
          /*False_Wall_East.Sections[1],*/ // rock section
          The_Minor_Erg.Sections[1],
          /*Pasty_Mesa.Sections[1],*/ // rock section
          South_Mesa.Sections[2],
        ], [
          // 6
          /*False_Wall_East.Sections[2],*/ // rock section
          The_Minor_Erg.Sections[2],
          /*Pasty_Mesa.Sections[2],*/ // rock section
          Red_Chasm.Sections[0],
        ], [
          // 7
          /*False_Wall_East.Sections[3],*/ // rock section
          /*Shield_Wall.Sections[0],*/ // rock section
          The_Minor_Erg.Sections[3],
          /*Pasty_Mesa.Sections[3],*/ // rock section
          Gara_Kulon.Sections[0],
        ], [
          // 8
          /*Imperial_Basin.Sections[0],*/ // family atomics
          /*False_Wall_East.Sections[4],*/ // rock section
          /*Shield_Wall.Sections[1],*/ // rock section
          Hole_In_The_Rock.Sections[0],
          /*Rim_Wall_West.Sections[0],*/ // rock section
          Basin.Sections[0],
          Sihaya_Ridge.Sections[0],
          Old_Gap.Sections[0],
        ], [
          // 9
          /*Imperial_Basin.Sections[1],*/ // family atomics
          /*Arrakeen.Sections[0],*/ // family atomics
          Old_Gap.Sections[1],
        ], [
          // 10
          Arsunt.Sections[0],
          /*Imperial_Basin.Sections[2],*/ // family atomics
          /*Carthag.Sections[0],*/ // family atomics
          Tsimpo.Sections[0],
          Broken_Land.Sections[0],
          Old_Gap.Sections[2],
        ], [
          // 11
          Arsunt.Sections[1],
          Hagga_Basin.Sections[0],
          Tsimpo.Sections[1],
          /*Plastic_Basin.Sections[0],*/ // rock section
          Broken_Land.Sections[1],
        ], [
          // 12
          Hagga_Basin.Sections[1],
          /*Plastic_Basin.Sections[1],*/ // rock section
          Tsimpo.Sections[2],
          Rock_Outcroppings.Sections[0],
        ], [
          // 13
          Wind_Pass.Sections[0],
          /*Plastic_Basin.Sections[2],*/ // rock section
          Bight_Of_The_Cliff.Sections[0],
          /*Sietch_Tabr.Sections[0],*/ // strongholds
          Rock_Outcroppings.Sections[1],
        ], [
          // 14
          Wind_Pass.Sections[1],
          The_Great_Flat.Sections[0],
          Funeral_Plain.Sections[0],
          Bight_Of_The_Cliff.Sections[1],
        ], [
          // 15
          Wind_Pass.Sections[2],
          The_Greater_Flat.Sections[0],
          /*False_Wall_West.Sections[0],*/ // rock section
          Habbanya_Erg.Sections[0],
        ], [
          // 16
          Wind_Pass_North.Sections[0],
          Wind_Pass.Sections[3],
          /*False_Wall_West.Sections[1],*/ // rock section
          Habbanya_Erg.Sections[1],
          Habbanya_Ridge_Flat.Sections[0],
          /*Habbanya_Sietch.Sections[0],*/ // strongholds
        ], [
          // 17
          Wind_Pass_North.Sections[1],
          Cielago_West.Sections[0],
          /*False_Wall_West.Sections[2],*/ // rock section
          Habbanya_Ridge_Flat.Sections[1],
        ],
      ];

      #endregion

      Influenced_By_Family_Atomics = [
        Imperial_Basin.Sections[0],
        Imperial_Basin.Sections[1],
        Arrakeen.Sections[0],
        Imperial_Basin.Sections[2],
        Carthag.Sections[0],
      ];
    }

    public Map() {
      Storm_Sector = 0;

      Shield_Wall_Was_Destroyed = false;
    }

    [JsonConstructor]
    public Map(int storm_sector, bool shield_wall_was_destroyed, IEnumerable<Section_Forces> section_forces_list, IEnumerable<int> spice_list) {
      Storm_Sector = storm_sector;

      ((Action)(shield_wall_was_destroyed switch {
        true => Destroy_Shield_Wall,
        false => () => { shield_wall_was_destroyed = false; }
      })).Invoke();

      int index = 0;
      section_forces_list.ForEach(forces => index++.To_Section().Copy_Forces_From(forces));

      index = 0;
      spice_list.ForEach(spice => index++.To_Section_With_Spice().Copy_Spice_From(spice));
    }

    #region Territories
    // territories have been initialised in increasing order by earliest sector crossed
    // if more territories start in the same sector, the one closer to the polar sink gets priority

    [JsonIgnore] static public Sand Cielago_North { get; } = new("Cielago North", 0, [None, None, 8]);
    [JsonIgnore] static public Sand Cielago_Depression { get; } = new("Cielago Depression", 0, 3);
    [JsonIgnore] static public Sand Meridian { get; } = new("Meridian", 0, 2);

    [JsonIgnore] static public Sand Cielago_South { get; } = new("Cielago South", 1, [12, None]);

    [JsonIgnore] static public Sand Cielago_East { get; } = new("Cielago East", 2, 2);

    [JsonIgnore] static public Sand Harg_Pass { get; } = new("Harg Pass", 3, 2);
    [JsonIgnore] static public Rock False_Wall_South { get; } = new("False Wall South", 3, 2);
    [JsonIgnore] static public Sand South_Mesa { get; } = new("South Mesa", 3, [None, 10, None]);

    [JsonIgnore] static public Rock False_Wall_East { get; } = new("False_Wall_East", 4, 5);
    [JsonIgnore] static public Sand The_Minor_Erg { get; } = new("The Minor Erg", 4, [None, None, None, 8]);
    [JsonIgnore] static public Rock Pasty_Mesa { get; } = new("Pasty_Mesa", 4, 4);
    [JsonIgnore] static public Strongholds Tuek_s_Sietch { get; } = new("Tuek's Sietch", 4);


    [JsonIgnore] static public Sand Red_Chasm { get; } = new("Red Chasm", 6, [8]);

    [JsonIgnore] static public Rock Shield_Wall { get; } = new("Shield_Wall", 7, 2);
    [JsonIgnore] static public Sand Gara_Kulon { get; } = new("Gara Kulon", 7, 1);

    [JsonIgnore] static public Sand Imperial_Basin { get; } = new("Imperial Basin", 8, 3);
    [JsonIgnore] static public Sand Hole_In_The_Rock { get; } = new("Hole In The Rock", 8, 1);
    [JsonIgnore] static public Rock Rim_Wall_West { get; } = new("Rim Wall West", 8, 1);
    [JsonIgnore] static public Sand Basin { get; } = new("Basin", 8, 1);
    [JsonIgnore] static public Sand Sihaya_Ridge { get; } = new("Sihaya Ridge", 8, [6]);
    [JsonIgnore] static public Sand Old_Gap { get; } = new("Old Gap", 8, [None, 6, None]);

    [JsonIgnore] static public Strongholds Arrakeen { get; } = new("Arrakeen", 9);

    [JsonIgnore] static public Sand Arsunt { get; } = new("Arsunt", 10, 2);
    [JsonIgnore] static public Strongholds Carthag { get; } = new("Carthag", 10);
    [JsonIgnore] static public Sand Tsimpo { get; } = new("Tsimpo", 10, 3);
    [JsonIgnore] static public Sand Broken_Land { get; } = new("Broken Land", 10, [None, 8]);

    [JsonIgnore] static public Sand Hagga_Basin { get; } = new("Hagga_Basin", 11, [None, 6]);
    [JsonIgnore] static public Rock Plastic_Basin { get; } = new("Plastic Basin", 11, 3);

    [JsonIgnore] static public Sand Rock_Outcroppings { get; } = new("Rock Outcroppings", 12, [None, 6]);

    [JsonIgnore] static public Sand Wind_Pass { get; } = new("Wind Pass", 13, 4);
    [JsonIgnore] static public Strongholds Sietch_Tabr { get; } = new("Sietch Tabr", 13);
    [JsonIgnore] static public Sand Bight_Of_The_Cliff { get; } = new("Bight Presence_Of The Cliff", 13, 2);

    [JsonIgnore] static public Sand The_Great_Flat { get; } = new("The Great Flat", 14, [10]);
    [JsonIgnore] static public Sand Funeral_Plain { get; } = new("Funeral Plain", 14, [6]);

    [JsonIgnore] static public Sand The_Greater_Flat { get; } = new("The Greater Flat", 15, 1);
    [JsonIgnore] static public Rock False_Wall_West { get; } = new("False Wall West", 15, 3);
    [JsonIgnore] static public Sand Habbanya_Erg { get; } = new("Habbanya Erg", 15, [8, None]);

    [JsonIgnore] static public Sand Wind_Pass_North { get; } = new("Wind Pass North", 16, [6, None]);
    [JsonIgnore] static public Sand Habbanya_Ridge_Flat { get; } = new("Habbanya Ridge Flat", 16, [None, 10]);
    [JsonIgnore] static public Strongholds Habbanya_Sietch { get; } = new("Habbanya Sietch", 16);

    [JsonIgnore] static public Sand Cielago_West { get; } = new("Cielago West", 17, 2);

    [JsonIgnore] static public Polar_Sink Polar_Sink { get; } = new("Polar Sink");

    #endregion

    #region Containers for Territories and Sections + List of Presences/Spice for serialization

    [JsonIgnore]
    static public IReadOnlyList<Territory> Territories { get; }

    [JsonIgnore]
    static public IReadOnlyList<Section> Sections { get; }

    [JsonIgnore]
    static public IReadOnlyList<With_Spice> Sections_With_Spice { get; }

    [JsonInclude]
    private IEnumerable<Section_Forces> Section_Forces_list => Sections.Select(section => section.Forces);

    [JsonInclude]
    private IEnumerable<int> Spice_List => Sections_With_Spice.Select(section => section.Spice_Avaliable);

    #endregion

    #region Sectors Stuff

    [JsonIgnore]
    public const int NUMBER_OF_SECTORS = 18;

    #endregion

    #region Storm
    
    [JsonIgnore]
    static public IReadOnlyList<System.Collections.Generic.HashSet<Section>> Storm_Affectable { get; }

    public int Storm_Sector { get; private set; }

    public void Move_Storm_Sector_Forward(int sectors_to_move) =>
      Storm_Sector = (Storm_Sector + sectors_to_move).To_Sector();

    public void Move_Storm(int sectors_to_move) {
      Enumerable.Range(Storm_Sector + 1, sectors_to_move).ToList().ForEach(pos =>
        Storm_Affectable[pos].ForEach(section => {
          Section_Forces graveyard = new(); // !!! Replace with actual graveyard !!!
          section.Forces.Remove_By_Storm(graveyard);
          section.Delete_Spice();
        })
      );
      Move_Storm_Sector_Forward(sectors_to_move);
    }

    #endregion

    #region Family Atomics

    public bool Shield_Wall_Was_Destroyed { get; private set; }

    [JsonIgnore]
    static private IReadOnlyCollection<Section> Influenced_By_Family_Atomics { get; }

    public void Destroy_Shield_Wall() {
      Shield_Wall_Was_Destroyed = true;
      // basically adds the imperial basin, arrakeen and carthag to the sections affected by the storm
      Influenced_By_Family_Atomics.ForEach(section => {
        Storm_Affectable[section.Origin_Sector].Add(section);
      });
    }

    #endregion

    #region Section Linking Methods

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

    #endregion

    /*public IEnumerable<Section> Get_Accessible_Sections(Faction faction, Section origin, uint depth) {
      if (origin.Is_Present(faction)) {
        throw new ArgumentException("The Faction \"" + faction.ToString() + "\" does not have any troops in this section.");
      }
      Queue<Section> q = [];
      q.Enqueue(origin);
      while (q.Active_Forces_Count > 0) {
        var current 
      }
      ICollection<Section> to_return = [];
      return to_return;
    }*/

    public bool Is_Accessible(Faction_Class faction, Section section) {
      return section.Origin_Sector != Storm_Sector &&
        !(section.Is_Full_Strongholds && !section.Forces.Is_Present(faction));
    }
  }
}
