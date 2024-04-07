﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace dune_library.Map {
  internal class Sand : Region {
    public Sand(string name, List<(ushort sector, ushort? spice_capacity)> data_list) : base(name) {
      {
        var first_sector = data_list.First().sector;
        var last_sector = data_list.Last().sector;
        var sectors = data_list.Select(sector => sector.Item1);
        var correct_sectors = Enumerable.Range(first_sector, last_sector + 1).Select(Map.To_Sector);
        if (Enumerable.SequenceEqual(correct_sectors, sectors) == false) {
          throw new ArgumentException("the sectors in '" + nameof(data_list) +
                                      "' must be in increasing order (how they were passed: " + sectors +
                                      ", how they should've been passed: " + correct_sectors + ")",
                                      nameof(data_list));
        }
      }
      sections = data_list.Select(e =>
                            e.spice_capacity.HasValue ? new Section(e.sector, (ushort)e.spice_capacity) : new Section(e.sector)
                          ).ToList();
    }

    public Sand(string name, ushort first_sector, List<ushort?> spice_capacities) : base(name) {
      ushort current_sector = first_sector;
      sections = spice_capacities.Select(spice_capacity =>
                                   spice_capacity.HasValue ?
                                    new Section(Map.To_Sector(current_sector++), spice_capacity.Value)
                                  :
                                    new Section(Map.To_Sector(current_sector++))
                                 ).ToList();
    }

    public Sand(string name, ushort first_sector, ushort sectors_spanned) : base(name) {
      sections = Enumerable.Range(first_sector, first_sector + sectors_spanned).Select(sector => new Section(Map.To_Sector(sector))).ToList();
    }
  }
}