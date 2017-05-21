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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Globalization;

namespace SanusVitae
{
    /// <summary>
    /// Interaction logic for СуточнаяЭкскреция.xaml
    /// </summary>
    public partial class DailyExcreationTestPage : UserControl, IDataContextProvider<DailyExcreationTest>
    {
        public DailyExcreationTestPage()
        {
            InitializeComponent();
            ОкноПоказателей.DataContext = new DailyExcreationTest();
        }
        public DailyExcreationTest GetDataContext()
        {
            var test_data = (DailyExcreationTest)ОкноПоказателей.DataContext;
            var assembly_ = this.GetType().Assembly;
            using (Stream stream = assembly_.GetManifestResourceStream(this.GetType().Namespace + "med_data.ChemInfo.xml"))
            {
                var document_ = XDocument.Load(stream).Root.Elements("test").Where(x => x.Attribute("name").Value == "DailyExcreationTest").ToArray()[0];
                foreach (var indicator in typeof(DailyExcreationTest).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.IsDefined(typeof(TestInfo))))
                {
                    if (indicator.GetValue(test_data) == null)
                        continue;
                    double value = (double)indicator.GetValue(test_data);
                    double moles_mass = Double.Parse(document_.Elements("component").Where(x => x.Attribute("name").Value == indicator.Name).ToArray()[0].Attribute("moles_mass").Value, CultureInfo.InvariantCulture);
                    switch(ОкноПоказателей.Children.OfType<ComboBox>().Where(x => x.Name == "unity" + indicator.Name).ToArray()[0].SelectedIndex)
                    {
                        case 1:
                            indicator.SetValue(test_data, (double)(value / moles_mass));
                            break;
                        case 2:
                            indicator.SetValue(test_data, (double)(value * 1000.0 / moles_mass));
                            break;
                    }
                }
            }
            return test_data;
        }

        public DailyExcreationTest ProvideDataContext()
        {
            return GetDataContext();
        }
    }
}
