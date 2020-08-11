using CoreWPF.Utilites.Navigation;
using DanceRegUltra.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels.CategoryMenuElements
{
    public class StyleMenuElementViewModel : NavigationModel
    {
        public StyleMenuElementViewModel(NavigationManager nav) : base(CategoryType.Style.ToString(), nav)
        {

        }

        public override void OnNavigatingFrom()
        {

        }

        public override void OnNavigatingTo(object arg)
        {

        }
    }
}
