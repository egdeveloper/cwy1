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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SanusVitae
{
    /// <summary>
    /// Interaction logic for UreaStoneTestPage.xaml
    /// </summary>
    public partial class UreaStoneTestPage : UserControl, IDataContextProvider<UreaStoneTest>
    {
        private UreaStoneTest test_;
        public UreaStoneTestPage()
        {
            InitializeComponent();
            test_ = new UreaStoneTest();
            gridTestIndicators.DataContext = test_;
        }

        public UreaStoneTest ProvideDataContext()
        {
            return (UreaStoneTest)gridTestIndicators.DataContext;
        }
    }
}
