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
    /// Interaction logic for БиохимическийАнализКрови.xaml
    /// </summary>
    public partial class BiochemBloodTestPage : UserControl, IDataContextProvider<BioChemTest>
    {
        public BiochemBloodTestPage()
        {
            InitializeComponent();
            ОкноПоказаталей.DataContext = new BioChemTest();
        }
        public BioChemTest GetDataContext()
        {
            return (BioChemTest)ОкноПоказаталей.DataContext;
        }

        public BioChemTest ProvideDataContext()
        {
            return GetDataContext();
        }
    }
}
