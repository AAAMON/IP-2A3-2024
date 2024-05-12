using EnumsNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Decks.Treachery {
  [Flags]
  public enum Treachery_Card {
    [Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Projectile)]
    Crysknife,
    [Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Projectile)]
    Maula_Pistol,
    [Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Projectile)]
    Slip_Tip,
    [Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Projectile)]
    Stunner,

    [Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Poison)]
    Chaumas,
    [Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Poison)]
    Chaumurky,
    [Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Poison)]
    Ellaca_Drug,
    [Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Poison)]
    Gom_Jabbar,

    [Card_Type(Card_Type.Weapon), Weapon_Type(Weapon_Type.Special)]
    Lasgun,

    [Card_Type(Card_Type.Defense), Weapon_Type(Weapon_Type.Projectile)]
    Shield,
    [Card_Type(Card_Type.Defense), Weapon_Type(Weapon_Type.Poison)]
    Snooper,

    [Card_Type(Card_Type.Other)]
    Cheap_Hero,
    [Card_Type(Card_Type.Other)]
    Family_Atomics,
    [Card_Type(Card_Type.Other)]
    Hajr,
    [Card_Type(Card_Type.Other)]
    Karama,
    [Card_Type(Card_Type.Other)]
    Tleilaxu_Ghola,
    [Card_Type(Card_Type.Other)]
    Truthtrance,
    [Card_Type(Card_Type.Other)]
    Weather_Control,

    [Card_Type(Card_Type.Worthless)]
    Baliset,
    [Card_Type(Card_Type.Worthless)]
    Jubba_Cloak,
    [Card_Type(Card_Type.Worthless)]
    Kulon,
    [Card_Type(Card_Type.Worthless)]
    La_La_La,
    [Card_Type(Card_Type.Worthless)]
    Trip_To_Gamont,
  }

  public enum Card_Type {
    Weapon,
    Defense,
    Worthless,
    Other,
  }

  [AttributeUsage(AttributeTargets.Field)]
  class Card_TypeAttribute : Attribute {

    public Card_Type Card_Type { get; }

    public Card_TypeAttribute(Card_Type card_type) {
      Card_Type = card_type;
    }
  }

  public enum Weapon_Type {
    Projectile,
    Poison,
    Special,
  }

  [AttributeUsage(AttributeTargets.Field)]
  class Weapon_TypeAttribute : Attribute {

    public Weapon_Type Weapon_Type { get; }

    public Weapon_TypeAttribute(Weapon_Type weapon_type) {
      Weapon_Type = weapon_type;
    }
  }

  public enum Defense_Type {
    Projectile,
    Poison,
  }

  [AttributeUsage(AttributeTargets.Field)]
  class Defense_TypeAttribute : Attribute {

    public Defense_Type Defense_Type { get; }

    public Defense_TypeAttribute(Defense_Type weapon_type) {
      Defense_Type = weapon_type;
    }
  }
}
