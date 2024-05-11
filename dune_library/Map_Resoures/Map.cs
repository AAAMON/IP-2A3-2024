using dune_library.Player_Resources;
using dune_library.Utils;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using dune_library.Map_Resoures;
using System.Text.Json.Serialization;
using static System.Collections.Specialized.BitVector32;
using Extensions = dune_library.Utils.Extensions;

// storm sectors go from 0 to 17, the polar sink is considered to be the 18 th storm sector
// 0 is considered to be the starting sector in the rulebook (aka the one with 8 spice capacity in the cielago north region)
namespace dune_library.Map_Resources {
  public class Map {
    public Map() {
      #region Territories Initialization
      // territories have been initialised in increasing order by earliest sector crossed
      // if more territories start in the same sector, the one closer to the polar sink gets priority
      uint territory_counter = 0;
      uint section_counter = 0;

      Cielago_North = new("Cielago North", 0, [None, None, 8], territory_counter++, ref section_counter);
      Cielago_Depression = new("Cielago Depression", 0, 3, territory_counter++, ref section_counter);
      Meridian = new("Meridian", 0, 2, territory_counter++, ref section_counter);

      Cielago_South = new("Cielago South", 1, [12, None], territory_counter++, ref section_counter);

      Cielago_East = new("Cielago East", 2, 2, territory_counter++, ref section_counter);

      Harg_Pass = new("Harg Pass", 3, 2, territory_counter++, ref section_counter);
      False_Wall_South = new("False Wall South", 3, 2, territory_counter++, ref section_counter);
      South_Mesa = new("South Mesa", 3, [None, 10, None], territory_counter++, ref section_counter);

      False_Wall_East = new("False_Wall_East", 4, 5, territory_counter++, ref section_counter);
      The_Minor_Erg = new("The Minor Erg", 4, [None, None, None, 8], territory_counter++, ref section_counter);
      Pasty_Mesa = new("Pasty_Mesa", 4, 4, territory_counter++, ref section_counter);
      Tuek_s_Sietch = new("Tuek's Sietch", 4, territory_counter++, ref section_counter);


      Red_Chasm = new("Red Chasm", 6, [8], territory_counter++, ref section_counter);

      Shield_Wall = new("Shield_Wall", 7, 2, territory_counter++, ref section_counter);
      Gara_Kulon = new("Gara Kulon", 7, 1, territory_counter++, ref section_counter);

      Imperial_Basin = new("Imperial Basin", 8, 3, territory_counter++, ref section_counter);
      Hole_In_The_Rock = new("Hole In The Rock", 8, 1, territory_counter++, ref section_counter);
      Rim_Wall_West = new("Rim Wall West", 8, 1, territory_counter++, ref section_counter);
      Basin = new("Basin", 8, 1, territory_counter++, ref section_counter);
      Sihaya_Ridge = new("Sihaya Ridge", 8, [6], territory_counter++, ref section_counter);
      Old_Gap = new("Old Gap", 8, [None, 6, None], territory_counter++, ref section_counter);

      Arrakeen = new("Arrakeen", 9, territory_counter++, ref section_counter);

      Arsunt = new("Arsunt", 10, 2, territory_counter++, ref section_counter);
      Carthag = new("Carthag", 10, territory_counter++, ref section_counter);
      Tsimpo = new("Tsimpo", 10, 3, territory_counter++, ref section_counter);
      Broken_Land = new("Broken Land", 10, [None, 8], territory_counter++, ref section_counter);

      Hagga_Basin = new("Hagga_Basin", 11, [None, 6], territory_counter++, ref section_counter);
      Plastic_Basin = new("Plastic Basin", 11, 3, territory_counter++, ref section_counter);

      Rock_Outcroppings = new("Rock Outcroppings", 12, [None, 6], territory_counter++, ref section_counter);

      Wind_Pass = new("Wind Pass", 13, 4, territory_counter++, ref section_counter);
      Sietch_Tabr = new("Sietch Tabr", 13, territory_counter++, ref section_counter);
      Bight_Of_The_Cliff = new("Bight Presence_Of The Cliff", 13, 2, territory_counter++, ref section_counter);

      The_Great_Flat = new("The Great Flat", 14, [10], territory_counter++, ref section_counter);
      Funeral_Plain = new("Funeral Plain", 14, [6], territory_counter++, ref section_counter);

      The_Greater_Flat = new("The Greater Flat", 15, 1, territory_counter++, ref section_counter);
      False_Wall_West = new("False Wall West", 15, 3, territory_counter++, ref section_counter);
      Habbanya_Erg = new("Habbanya Erg", 15, [8, None], territory_counter++, ref section_counter);

      Wind_Pass_North = new("Wind Pass North", 16, [6, None], territory_counter++, ref section_counter);
      Habbanya_Ridge_Flat = new("Habbanya Ridge Flat", 16, [None, 10], territory_counter++, ref section_counter);
      Habbanya_Sietch = new("Habbanya Sietch", 16, territory_counter++, ref section_counter);

      Cielago_West = new("Cielago West", 17, 2, territory_counter++, ref section_counter);

      Polar_Sink = new("Polar Sink", territory_counter++, ref section_counter);

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

      Spice_Dict = new Dictionary<uint, Capacity_Avaliable_Pair>(Sections_With_Spice.Select(section =>
        new KeyValuePair<uint, Capacity_Avaliable_Pair>(section.Id, new(section))
      ));


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

      Storm_Sector = 0;

      Shield_Wall_Was_Destroyed = false;
    }

    #region Territories
    // territories have been initialised in increasing order by earliest sector crossed
    // if more territories start in the same sector, the one closer to the polar sink gets priority

    [JsonIgnore] public Sand Cielago_North { get; }
    [JsonIgnore] public Sand Cielago_Depression { get; }
    [JsonIgnore] public Sand Meridian { get; }

    [JsonIgnore] public Sand Cielago_South { get; }

    [JsonIgnore] public Sand Cielago_East { get; }

    [JsonIgnore] public Sand Harg_Pass { get; }
    [JsonIgnore] public Rock False_Wall_South { get; }
    [JsonIgnore] public Sand South_Mesa { get; }

    [JsonIgnore] public Rock False_Wall_East { get; }
    [JsonIgnore] public Sand The_Minor_Erg { get; }
    [JsonIgnore] public Rock Pasty_Mesa { get; }
    [JsonIgnore] public Strongholds Tuek_s_Sietch { get; }


    [JsonIgnore] public Sand Red_Chasm { get; }

    [JsonIgnore] public Rock Shield_Wall { get; }
    [JsonIgnore] public Sand Gara_Kulon { get; }

    [JsonIgnore] public Sand Imperial_Basin { get; }
    [JsonIgnore] public Sand Hole_In_The_Rock { get; }
    [JsonIgnore] public Rock Rim_Wall_West { get; }
    [JsonIgnore] public Sand Basin { get; }
    [JsonIgnore] public Sand Sihaya_Ridge { get; }
    [JsonIgnore] public Sand Old_Gap { get; }

    [JsonIgnore] public Strongholds Arrakeen { get; }

    [JsonIgnore] public Sand Arsunt { get; }
    [JsonIgnore] public Strongholds Carthag { get; }
    [JsonIgnore] public Sand Tsimpo { get; }
    [JsonIgnore] public Sand Broken_Land { get; }

    [JsonIgnore] public Sand Hagga_Basin { get; }
    [JsonIgnore] public Rock Plastic_Basin { get; }

    [JsonIgnore] public Sand Rock_Outcroppings { get; }

    [JsonIgnore] public Sand Wind_Pass { get; }
    [JsonIgnore] public Strongholds Sietch_Tabr { get; }
    [JsonIgnore] public Sand Bight_Of_The_Cliff { get; }

    [JsonIgnore] public Sand The_Great_Flat { get; }
    [JsonIgnore] public Sand Funeral_Plain { get; }

    [JsonIgnore] public Sand The_Greater_Flat { get; }
    [JsonIgnore] public Rock False_Wall_West { get; }
    [JsonIgnore] public Sand Habbanya_Erg { get; }

    [JsonIgnore] public Sand Wind_Pass_North { get; }
    [JsonIgnore] public Sand Habbanya_Ridge_Flat { get; }
    [JsonIgnore] public Strongholds Habbanya_Sietch { get; }

    [JsonIgnore] public Sand Cielago_West { get; }

    [JsonIgnore] public Polar_Sink Polar_Sink { get; }

    #endregion

    #region Containers for Territories and Sections + List of Presences/Spice for serialization

    public IReadOnlyList<Territory> Territories { get; }

    [JsonIgnore] public IReadOnlyList<Section> Sections { get; }

    [JsonIgnore] public IReadOnlyList<With_Spice> Sections_With_Spice { get; }
    private class Capacity_Avaliable_Pair {
      private readonly With_Spice section;
      public Capacity_Avaliable_Pair(With_Spice section) {
        this.section = section;
      }
      public uint Capacity => section.Spice_Capacity;
      public uint Avaliable => section.Spice_Avaliable;
    }
    [JsonInclude] private IReadOnlyDictionary<uint, Capacity_Avaliable_Pair> Spice_Dict { get; }

    #endregion

    #region Sectors Stuff

    [JsonIgnore] public const uint NUMBER_OF_SECTORS = 18;

    #endregion

    #region Storm
    
    [JsonIgnore] public IReadOnlyList<System.Collections.Generic.HashSet<Section>> Storm_Affectable { get; }

    public uint Storm_Sector { get; private set; }

    public void Move_Storm_Sector_Forward(uint sectors_to_move) =>
      Storm_Sector = (Storm_Sector + sectors_to_move).To_Sector();

    public void Move_Storm(uint sectors_to_move) { /* !!! Move to Storm Phase, makes more sense for it to be there !!! */
      Extensions.Range(Storm_Sector + 1, sectors_to_move).ToList().ForEach(pos =>
        Storm_Affectable[(int)pos].ForEach(section => {
          Forces graveyard = new(); /* !!! Replace with actual graveyard !!! */
          section.Forces.Remove_By_Storm(graveyard);
          section.Delete_Spice();
        })
      );
      Move_Storm_Sector_Forward(sectors_to_move);
    }

    #endregion

    #region Family Atomics

    public bool Shield_Wall_Was_Destroyed { get; private set; }

    private IReadOnlyCollection<Section> Influenced_By_Family_Atomics { get; }

    public void Destroy_Shield_Wall() {
      Shield_Wall_Was_Destroyed = true;
      // basically adds the imperial basin, arrakeen and carthag to the sections affected by the storm
      Influenced_By_Family_Atomics.ForEach(section => {
        Storm_Affectable[(int)section.Origin_Sector].Add(section);
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

    public bool Is_Accessible(Faction faction, Section section) {
      return section.Origin_Sector != Storm_Sector &&
        !(section.Is_Full_Strongholds && !section.Forces.Is_Present(faction));
    }

      public Territory To_Territory(uint id) {
        if (id > Territories.Count) {
          throw new ArgumentException("no territory is mapped to this id (max: " + Territories.Count + ", id: " + id + ")");
        }
        return Territories[(int)id];
      }
      public Section To_Section(uint id) {
        if (id > Sections.Count) {
          throw new ArgumentException("no section is mapped to this id (max: " + Sections.Count + ", id: " + id + ")");
        }
        return Sections[(int)id];
      }

      public With_Spice To_Section_With_Spice(uint id) {
        if (id > Sections_With_Spice.Count) {
          throw new ArgumentException("no section with spice is mapped to this id (max: " + Sections_With_Spice.Count + ", id: " + id + ")");
        }
        return Sections_With_Spice[(int)id];
      }
  }
}
