﻿using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace dune_library.Map_Resources {
  internal class Sand : Territory {
    public override IReadOnlyList<Section> Sections { get; }
    public Sand(string name, List<(int sector, Option<int> spice_capacity)> data_list) : base(name) {
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
      Sections = data_list.Select(e =>
                            e.spice_capacity.Match(
                              value => new Section(e.sector, this, value),
                              () => new Section(e.sector, this)
                            )
                          ).ToList();
    }

    public Sand(string name, int first_sector, List<Option<int>> spice_capacities) : base(name) {
      int current_sector = first_sector;
      Sections = spice_capacities.Select(spice_capacity =>
                                    spice_capacity.Match(
                                      value => new Section(Map.To_Sector(current_sector++), this, value),
                                      () => new Section(Map.To_Sector(current_sector++), this)
                                    )
                                  ).ToList();
    }

    public Sand(string name, int first_sector, int sectors_spanned) : base(name) {
      Sections = Enumerable.Range(first_sector, first_sector + sectors_spanned).Select(sector => new Section(Map.To_Sector(sector), this)).ToList();
    }
  }
}
