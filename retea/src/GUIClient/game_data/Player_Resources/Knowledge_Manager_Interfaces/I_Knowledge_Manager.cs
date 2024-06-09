using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Player_Resources.Knowledge_Manager_Interfaces {
  public interface I_Knowledge_Manager : I_Knowledge_Manager_Read_Only, I_Spice_Manager, I_Treachery_Cards_Manager, I_Traitors_Initializer {
  }
}
