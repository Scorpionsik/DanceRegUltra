using CoreWPF.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Interfaces
{
    public interface ICategoriesManagerCommands
    {
        RelayCommand Command_add { get; }
        RelayCommand Command_remove { get; }
    }
}
