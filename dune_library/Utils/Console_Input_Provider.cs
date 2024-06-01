using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dune_library.Utils
{
    public class Console_Input_Provider : I_Input_Provider
    {
        public async Task<string> GetInputAsync()
        {
            return await Task.Run(() => Console.ReadLine());
        }
    }
}
