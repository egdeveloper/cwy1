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
    /// Interaction logic for CommonUreaTestPage.xaml
    /// </summary>
    public partial class CommonUreaTestPage : UserControl, IDataContextProvider<CommonUreaTest>
    {
        private CommonUreaTest common_urea_test;
        public CommonUreaTestPage()
        {
            InitializeComponent();
            common_urea_test = new CommonUreaTest();
            gridIndicators.DataContext = common_urea_test;
        }

        public CommonUreaTest ProvideDataContext()
        {
            return (CommonUreaTest)gridIndicators.DataContext;
        }
    }
}
