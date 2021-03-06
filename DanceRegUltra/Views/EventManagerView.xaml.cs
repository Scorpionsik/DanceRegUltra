﻿using CoreWPF.Windows;
using DanceRegUltra.Models;
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
    /// Логика взаимодействия для EventManagerView.xaml
    /// </summary>
    public partial class EventManagerView : WindowExt
    {
        public EventManagerView(int idEventLoad)
        {
            InitializeComponent();
            this.DataContext = new EventManagerViewModel(idEventLoad);
        }
    }
}
