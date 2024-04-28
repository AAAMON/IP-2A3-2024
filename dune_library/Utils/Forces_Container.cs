using dune_library.Player_Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils {
  internal class Forces_Container {
    public uint Normal { get; private set; }

    public uint Special { get; private set; }

    public Forces_Container(uint normal, uint special) {
      Normal = normal;
      Special = special;
    }

    #region Add, Bene Gesserit swaps and Remove

    /// <summary>
    /// <para>
    /// Adds a certain number of normal and special(Fedaykin, Sardaukar, Advisors) forces to the container
    /// </para>
    /// </summary>
    /// <param name="normal">The amount of normal forces to be added</param>
    /// <param name="special">The amount of special forces to be added</param>
    public void Add(uint normal, uint special) {
      Normal += normal;
      Special += special;
    }

    public void Add(Forces_Container other) => Add(other.Normal, other.Special);

    /// <summary>
    /// <para>
    /// Converts the container's normal forces
    /// </para>
    /// <para>
    /// this is mainly used by Bene Gesserit players.
    /// </para>
    /// </summary>
    /// <param name="faction"></param>
    public void Flip_Normal_To_Special() {
      Special += Normal;
      Normal = 0;
    }

    /// <summary>
    /// <para>
    /// Converts the special forces in this player's presence to normal forces.
    /// </para>
    /// <para>
    /// this is mainly used by Bene Gesserit players.
    /// </para>
    /// </summary>
    /// <param name="faction"></param>
    public void Flip_Special_To_Normal() {
      Normal += Special;
      Special = 0;
    }

    /// <summary>
    /// <para>
    /// Swaps the special and normal forces in this player's presence.
    /// </para>
    /// <para>
    /// this is mainly used by Bene Gesserit players.
    /// </para>
    /// </summary>
    /// <param name="faction"></param>
    public void Swap() {
      uint aux = Normal;
      Normal = Special;
      Special = aux;
    }

    /// <summary>
    /// <para>
    /// Removes a certain number of normal and special(Fedaykin, Sardaukar, Advisors) forces from the container
    /// </para>
    /// </summary>
    /// <param name="normal">The amount of normal forces to be removed</param>
    /// <param name="special">The amount of special forces to be removed</param>
    /// <exception cref="ArgumentException">
    /// the number of normal/special forces to be removed exceeds the number normal/special forces present in the container
    /// </exception>
    public void Remove(uint normal, uint special) {
      if (Normal < normal) {
        throw new ArgumentException("Tried to remove " + normal + " normal forces, while there are " + Normal + " normal forces present", nameof(normal));
      }
      if (Special < special) {
        throw new ArgumentException("Tried to remove " + special + " special forces, while there are " + Special + " special forces present", nameof(special));
      }
      Normal -= normal;
      Special -= special;
    }

    public void Remove(Forces_Container other) => Remove(other.Normal, other.Special);

    public void Remove_Half() {
      Normal /= 2;
      Special /= 2;
    }

    public void Remove() {
      (uint n, uint s) = this;
      Remove(n, s);
    }

    #endregion

    #region Conditions

    public bool Is_Empty => Normal == 0 && Special == 0;

    public static bool operator ==(Forces_Container a, Forces_Container b) {
      if (ReferenceEquals(a, b)) {
        return true;
      }
      if (a is null || b is null) {
        return false;
      }
      return a.Equals(b);
    }

    public static bool operator !=(Forces_Container a, Forces_Container b) => !(a == b);

    public override bool Equals(object obj) {
      if (obj is Forces_Container other) {
        return Normal == other.Normal && Special == other.Special;
      }
      return false;
    }

    #endregion

    public override int GetHashCode() => ((object)this).GetHashCode();

    internal void Deconstruct(out uint normal, out uint special) {
      normal = Normal;
      special = Special;
    }

    public Forces_Container Clone => new(Normal, Special); // ICloneable makes you cast

    public static implicit operator Forces_Container((uint normal, uint special) value) => new Forces_Container(value.normal, value.special);
  }
}
