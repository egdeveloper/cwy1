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
    /// Interaction logic for CommonBloodTest.xaml
    /// </summary>
    public partial class CommonBloodTestPage : UserControl, IDataContextProvider<CommonBloodTest>
    {
        private CommonBloodTest cm_blood_test;
        public CommonBloodTestPage()
        {
            InitializeComponent();
            cm_blood_test = new CommonBloodTest();
            ОкноПоказателей.DataContext = cm_blood_test;
        }
        public CommonBloodTest GetDataContext()
        {
            return (CommonBloodTest)ОкноПоказателей.DataContext;
        }

        public CommonBloodTest ProvideDataContext()
        {
            return GetDataContext();
        }
    }
}
