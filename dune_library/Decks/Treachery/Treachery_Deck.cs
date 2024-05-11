using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Treachery_Cards {
  public class Treachery_Deck {
        public IList<Treachery_Card> treachery_deck;
        public Treachery_Deck()
        {
            IList<Treachery_Card> default_treachery_deck = [
                new Treachery_Card("Crysknife","Weapon", "Projectile"),
                new Treachery_Card("Maula Pistol", "Weapon", "Projectile"),
                new Treachery_Card("Slip Tip", "Weapon", "Projectile"),
                new Treachery_Card("Stunner", "Weapon", "Projectile"),
                new Treachery_Card("Chaumas", "Weapon", "Poison"),
                new Treachery_Card("Chaumurky", "Weapon", "Poison"),
                new Treachery_Card("Ellaca Drug", "Weapon", "Poison"),
                new Treachery_Card("Gom Jabbar", "Weapon", "Poison"),
                new Treachery_Card("Lasgun", "Weapon", "Special"),
                new Treachery_Card("Shield", "Defense", "Projectile"),
                new Treachery_Card("Snooper", "Defense", "Poison"),
                new Treachery_Card("Cheap Hero", "Special", "Lider"),
                new Treachery_Card("Family Atomics", "Special", "Storm"),
                new Treachery_Card("Hajr", "Special", "Movement"),
                new Treachery_Card("Karama", "Special", "Other"),
                new Treachery_Card("Tleilaxu Ghola", "Special", "Other"),
                new Treachery_Card("Truthtrance", "Special", " Other"),
                new Treachery_Card("Weather Control", "Special", "Storm"),
                new Treachery_Card("Baliset", "Worthless", "Other"),
                new Treachery_Card("Jubba Cloak", "Worthless", "Other"),
                new Treachery_Card("Kulon", "Worthless", "Other"),
                new Treachery_Card("La, La, La", "Worthless", "Other"),
                new Treachery_Card("Trip to Gamont", "Worthless", "Other")
            ];

            /* one way to shuffle a deck
            Random rnd = new Random();
            while(default_treachery_deck.Count() > 0) {
                treachery_deck.Add(default_treachery_deck[rnd.Next(default_treachery_deck.Count())]);
            }
            */
            treachery_deck = default_treachery_deck;
        }
        public Treachery_Card Read_First() => treachery_deck[0];
        
  }
}
