﻿using CoreWPF.Windows;
using DanceRegUltra.Enums;
using DanceRegUltra.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DanceRegUltra.Views
{
    /// <summary>
    /// Логика взаимодействия для CategoryManagerViewModel.xaml
    /// </summary>
    public partial class CategoryManagerView : DialogWindowExt
    {
        public CategoryManagerView(CategoryType select_type = CategoryType.League)
        {
            InitializeComponent();
            this.DataContext = new CategoryManagerViewModel(select_type, null);
        }
    }
}
