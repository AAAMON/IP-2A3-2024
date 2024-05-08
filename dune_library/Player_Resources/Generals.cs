using LanguageExt.ClassInstances.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources {
  public static class Generals {

    #region Atreides

    public static General Dr_Wellington_Yueh { get; } = new(Faction.Atreides, "Dr. Wellington Yueh", 1);
    public static General Duncan_Idaho { get; } = new(Faction.Atreides, "Duncan Idaho", 2);
    public static General Gurney_Halleck { get; } = new(Faction.Atreides, "Gurney Halleck", 4);
    public static General Lady_Jessica { get; } = new(Faction.Atreides, "Lady Jessica", 5);
    public static General Thufir_Hawat { get; } = new(Faction.Atreides, "Thufir Hawat", 5);

    #endregion

    #region Bene Gesserit

    public static General Wanna_Yueh { get; } = new (Faction.Bene_Gesserit, "Wanna Yueh", 5);
    public static General Princess_Irulan { get; } = new (Faction.Bene_Gesserit, "Princess Irulan", 5);
    public static General Mother_Ramallo { get; } = new (Faction.Bene_Gesserit, "Mother Ramallo", 5);
    public static General Margot_Lady_Fenring { get; } = new(Faction.Bene_Gesserit, "Margot Lady Fenring", 5);
    public static General Alia { get; } = new(Faction.Bene_Gesserit, "Alia", 5);

    #endregion

    #region Emperor

    public static General Bashar { get; } = new(Faction.Emperor, "Bashar", 2);
    public static General Burseg { get; } = new(Faction.Emperor, "Burseg", 3);
    public static General Caid { get; } = new(Faction.Emperor, "Caid", 3);
    public static General Captain_Aramsham { get; } = new (Faction.Emperor, "Captain Aramsham", 5);
    public static General Hasimir_Fenring { get; } = new (Faction.Emperor, "Hasimir Fenring", 6);

    #endregion

    #region Fremen

    public static General Jamis { get; } = new(Faction.Fremen, "Jamis", 2);
    public static General Shadout_Mapes { get; } = new (Faction.Fremen, "Shadout Mapes", 3);
    public static General Otheym { get; } = new (Faction.Fremen, "Otheym", 5);
    public static General Chani { get; } = new(Faction.Fremen, "Chani", 6);
    public static General Stilgar { get; } = new(Faction.Fremen, "Stilgar", 7);

    #endregion

    #region Spacing_Guild

    public static General Guild_Rep { get; } = new (Faction.Spacing_Guild, "Guild Rep.", 1);
    public static General Soo_Soo_Sook { get; } = new(Faction.Spacing_Guild, "Soo-Soo Sook", 2);
    public static General Esmar_Tuek { get; } = new (Faction.Spacing_Guild, "Esmar Tuek", 3);
    public static General Master_Bewt { get; } = new (Faction.Spacing_Guild, "Master Bewt", 3);
    public static General Staban_Tuek { get; } = new (Faction.Spacing_Guild, "Staban Tuek", 5);

    #endregion

    #region Harkonnen

    public static General Umman_Kudu { get; } = new (Faction.Harkonnen, "Umman Kudu", 1);
    public static General Captain_Iakin_Nefud { get; } = new(Faction.Harkonnen, "Captain Iakin Nefud", 2);
    public static General Piter_de_Vries { get; } = new(Faction.Harkonnen, "Piter de Vries", 3);
    public static General Beast_Rabban { get; } = new (Faction.Harkonnen, "Beast Rabban", 4);
    public static General Feyd_Rautha { get; } = new (Faction.Harkonnen, "Feyd-Rautha", 6);

    #endregion
  }
}
