﻿using CoreWPF.Utilites;
using DanceRegUltra.Models.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Interfaces
{
    public interface ICategoriesMenu
    {
        ListExt<CategoryString> Categorys { get; }

        CategoryString Select_category { get; set; }

        RelayCommand<string> Command_add { get; }
        RelayCommand Command_remove { get; }
    }
}
