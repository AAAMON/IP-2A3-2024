using dune_library.Utils;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Decks.Treachery {
  public static class Treachery_Cards {
    public enum Treachery_Card {
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Projectile)]
      Crysknife,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Projectile)]
      Maula_Pistol,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Projectile)]
      Slip_Tip,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Projectile)]
      Stunner,

      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Poison)]
      Chaumas,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Poison)]
      Chaumurky,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Poison)]
      Ellaca_Drug,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Poison)]
      Gom_Jabbar,

      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Special)]
      Lasgun,

      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Defense), Defense_Type(Defense_Type.Projectile)]
      Shield,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Defense), Defense_Type(Defense_Type.Poison)]
      Snooper,

      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Other)]
      Cheap_Hero,
      [Remove_From_The_Game_After_Use(true), Card_Type(Card_Type.Other)]
      Family_Atomics,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Other)]
      Hajr,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Other)]
      Karama,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Other)]
      Tleilaxu_Ghola,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Other)]
      Truthtrance,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Other)]
      Weather_Control,

      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Worthless)]
      Baliset,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Worthless)]
      Jubba_Cloak,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Worthless)]
      Kulon,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Worthless)]
      La_La_La,
      [Remove_From_The_Game_After_Use(false), Card_Type(Card_Type.Worthless)]
      Trip_To_Gamont,
    }

    #region attributes

    private enum Card_Type {
      Weapon,
      Defense,
      Worthless,
      Other,
    }

    [AttributeUsage(AttributeTargets.Field)]
    private class Card_TypeAttribute : Attribute {

      public Card_Type Card_Type { get; }

      public Card_TypeAttribute(Card_Type card_type) {
        Card_Type = card_type;
      }
    }

    private enum Weapon_Type {
      Projectile,
      Poison,
      Special,
    }

    [AttributeUsage(AttributeTargets.Field)]
    private class Weapon_TypeAttribute : Attribute {

      public Weapon_Type Weapon_Type { get; }

      public Weapon_TypeAttribute(Weapon_Type weapon_type) {
        Weapon_Type = weapon_type;
      }
    }

    private enum Defense_Type {
      Projectile,
      Poison,
    }

    [AttributeUsage(AttributeTargets.Field)]
    private class Defense_TypeAttribute : Attribute {

      public Defense_Type Defense_Type { get; }

      public Defense_TypeAttribute(Defense_Type weapon_type) {
        Defense_Type = weapon_type;
      }
    }

    [AttributeUsage(AttributeTargets.Field)]
    private class Remove_From_The_Game_After_UseAttribute : Attribute {
      public bool Remove_From_Game_After_Use { get; }
      public Remove_From_The_Game_After_UseAttribute(bool remove_after_use) {
        Remove_From_Game_After_Use = remove_after_use;
      }
    }

    #endregion

    public static I_Occurence_Dict_Read_Only<Treachery_Card> Default_Treachery_Deck_Composition { get; } = new Occurence_Dict<Treachery_Card> {
      //projectile weapons
      [Treachery_Card.Crysknife] = 1,
      [Treachery_Card.Maula_Pistol] = 1,
      [Treachery_Card.Slip_Tip] = 1,
      [Treachery_Card.Stunner] = 1,

      //poison weapons
      [Treachery_Card.Chaumas] = 1,
      [Treachery_Card.Chaumurky] = 1,
      [Treachery_Card.Ellaca_Drug] = 1,
      [Treachery_Card.Gom_Jabbar] = 1,

      [Treachery_Card.Lasgun] = 1,

      [Treachery_Card.Shield] = 4,
      [Treachery_Card.Snooper] = 4,

      [Treachery_Card.Cheap_Hero] = 3,
      [Treachery_Card.Family_Atomics] = 1,
      [Treachery_Card.Hajr] = 1,
      [Treachery_Card.Karama] = 2,
      [Treachery_Card.Tleilaxu_Ghola] = 1,
      [Treachery_Card.Truthtrance] = 2,
      [Treachery_Card.Weather_Control] = 1,

      //worthless cards
      [Treachery_Card.Baliset] = 1,
      [Treachery_Card.Jubba_Cloak] = 1,
      [Treachery_Card.Kulon] = 1,
      [Treachery_Card.La_La_La] = 1,
      [Treachery_Card.Trip_To_Gamont] = 1,
    };
    
    public static bool Has_To_Be_Removed_From_The_Game_After_Use(this Treachery_Card card) => card.GetAttributes()!.Get<Remove_From_The_Game_After_UseAttribute>()!.Remove_From_Game_After_Use == true;

    public static bool Is_Weapon(this Treachery_Card card) => card.GetAttributes()!.Get<Card_TypeAttribute>()!.Card_Type == Card_Type.Weapon;

    public static bool Is_Projectile_Weapon(this Treachery_Card card) => card.GetAttributes()!.Get<Card_TypeAttribute>()!.Card_Type == Card_Type.Weapon && card.GetAttributes()!.Get<Weapon_TypeAttribute>()!.Weapon_Type == Weapon_Type.Projectile;

    public static bool Is_Poison_Weapon(this Treachery_Card card) => card.GetAttributes()!.Get<Card_TypeAttribute>()!.Card_Type == Card_Type.Weapon && card.GetAttributes()!.Get<Weapon_TypeAttribute>()!.Weapon_Type == Weapon_Type.Poison;

    public static bool Is_Special_Weapon(this Treachery_Card card) => card.GetAttributes()!.Get<Card_TypeAttribute>()!.Card_Type == Card_Type.Weapon && card.GetAttributes()!.Get<Weapon_TypeAttribute>()!.Weapon_Type == Weapon_Type.Special;

    public static bool Is_Lasgun(this Treachery_Card card) => card.Is_Special_Weapon();

    public static bool Is_Defense(this Treachery_Card card) => card.GetAttributes()!.Get<Card_TypeAttribute>()!.Card_Type == Card_Type.Defense;

    public static bool Is_Projectile_Defense(this Treachery_Card card) => card.GetAttributes()!.Get<Card_TypeAttribute>()!.Card_Type == Card_Type.Defense && card.GetAttributes()!.Get<Defense_TypeAttribute>()!.Defense_Type == Defense_Type.Projectile;

    public static bool Is_Poison_Defense(this Treachery_Card card) => card.GetAttributes()!.Get<Card_TypeAttribute>()!.Card_Type == Card_Type.Defense && card.GetAttributes()!.Get<Defense_TypeAttribute>()!.Defense_Type == Defense_Type.Poison;

    public static bool Is_Other(this Treachery_Card card) => card.GetAttributes()!.Get<Card_TypeAttribute>()!.Card_Type == Card_Type.Other;

    public static bool Is_Worthless(this Treachery_Card card) => card.GetAttributes()!.Get<Card_TypeAttribute>()!.Card_Type == Card_Type.Worthless;
  }
}
