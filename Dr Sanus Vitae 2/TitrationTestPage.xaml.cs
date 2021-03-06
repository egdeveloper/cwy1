﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SanusVitae
{
    /// <summary>
    /// Interaction logic for TitrationTestPage.xaml
    /// </summary>
    public partial class TitrationTestPage : UserControl, IDataContextProvider<TitrationTest>
    {
        public TitrationTestPage()
        {
            InitializeComponent();
            gridIndicators.DataContext = new TitrationTest();
        }

        public TitrationTest ProvideDataContext()
        {
            return (TitrationTest)gridIndicators.DataContext;
        }
    }
}
