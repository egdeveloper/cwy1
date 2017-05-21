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
using System.IO;
using System.Reflection;
using Novacode;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Controls.DataVisualization.Charting;

namespace SanusVitae
{
    /// <summary>
    /// Interaction logic for DynamicWindow.xaml
    /// </summary>
    public partial class DynamicsWindow : Window
    {
        private IEnumerable<MedicalTest> test_list_;
        private Type test_type;
        private IEnumerable<PropertyInfo> properties_;
        private PropertyInfo indicator_;
        private TestNorms norms_;
        private string indicator_unity;
        public DynamicsWindow()
        {
            InitializeComponent();
        }
        private void comboboxTestTypeDynamics_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                comboboxIndexType.Items.Clear();
                switch ((sender as ComboBox).SelectedIndex)
                {
                    case 0:
                        test_type = typeof(CommonBloodTest);
                        test_list_ = PatientDB.Instance.GetAnalysis(CurrentPatient.ID, test_type);
                        break;
                    case 1:
                        test_type = typeof(BioChemTest);
                        test_list_ = PatientDB.Instance.GetAnalysis(CurrentPatient.ID, test_type);
                        break;
                    case 2:
                        test_type = typeof(DailyExcreationTest);
                        test_list_ = PatientDB.Instance.GetAnalysis(CurrentPatient.ID, test_type);
                        break;
                    case 3:
                        test_type = typeof(UreaColorTest);
                        test_list_ = PatientDB.Instance.GetAnalysis(CurrentPatient.ID, test_type);
                        break;
                    case 4:
                        test_type = typeof(CommonUreaTest);
                        test_list_ = PatientDB.Instance.GetAnalysis(CurrentPatient.ID, test_type);
                        break;
                    case 5:
                        test_type = typeof(TitrationTest);
                        test_list_ = PatientDB.Instance.GetAnalysis(CurrentPatient.ID, test_type);
                        break;
                    case 6:
                        test_type = typeof(UreaStoneTest);
                        test_list_ = PatientDB.Instance.GetAnalysis(CurrentPatient.ID, test_type);
                        break;
                }
                if (test_list_.ToArray().Length <= 0)
                {
                    MessageBox.Show("У пациента нет динамики по данному анализу!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                DateTime min_date = test_list_.Min(x => x.Date);
                DateTime max_date = test_list_.Max(x => x.Date);
                test_list_ = from x in test_list_ where x.Date >= (datepickerFrom.SelectedDate ?? min_date) && x.Date <= (datepickerTo.SelectedDate ?? max_date) select x;
                properties_ = from x in test_type.GetProperties(BindingFlags.Public | BindingFlags.Instance) where Attribute.IsDefined(x, typeof(TestInfo)) select x;
                properties_ = properties_.Where(x => ((TestInfo)x.GetCustomAttribute<TestInfo>()).Visible);
                foreach (var property in properties_)
                    comboboxIndexType.Items.Add(property.GetCustomAttribute<TestInfo>().DisplayName);
                comboboxIndexType.IsEnabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(String.Format("Обнаружена ошибка при работе программы!\n{0}", exception.Message), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void contextMenuitemSaveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog save_fileDialog = new SaveFileDialog() { Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp|Word документ (*.docx)|*.docx" };
                if (save_fileDialog.ShowDialog() == true && !String.IsNullOrEmpty(save_fileDialog.FileName))
                {
                    string file_name = save_fileDialog.FileName;
                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)chartControl.ActualWidth, (int)chartControl.ActualHeight, 100, 100, PixelFormats.Pbgra32);
                    rtb.Render(chartControl);

                    BitmapEncoder bitmap_encoder = null;
                    if (file_name.Contains(".png"))
                        bitmap_encoder = new PngBitmapEncoder();
                    else if (file_name.Contains(".jpg"))
                        bitmap_encoder = new JpegBitmapEncoder();
                    else if (file_name.Contains(".bmp"))
                        bitmap_encoder = new BmpBitmapEncoder();

                    else if (file_name.Contains(".docx"))
                    {
                        DocumentGenerator.CreateDynamicsReport(CurrentPatient, test_list_, test_type, indicator_, file_name);
                        MessageBox.Show("Документ с отчетом о динамики показателей успешно сохранен!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    bitmap_encoder.Frames.Add(BitmapFrame.Create(rtb));
                    using (FileStream fs = new FileStream(save_fileDialog.FileName, FileMode.Create))
                    {
                        bitmap_encoder.Save(fs);
                        MessageBox.Show("График динамики показателей успешно сохранен!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch(Exception exception)
            {
                string message = String.Format("Обнаружена проблема при работе программы!\n{0}", exception.Message);
                MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void contextMenuItemCopyIntoClipboard_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)chartControl.ActualWidth, (int)chartControl.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(chartControl);
            Clipboard.SetImage(rtb);
        }
        public Patient CurrentPatient { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = CurrentPatient.ToString();
        }

        private void buttonPlotGraph_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (comboboxIndexType.SelectedIndex >= 0)
                {
                    if (test_list_.Count() <= 0)
                        return;
                    indicator_ = properties_.ToArray()[comboboxIndexType.SelectedIndex];

                    chartControl.Title = indicator_.GetCustomAttribute<TestInfo>().DisplayName;
                    indicator_unity = indicator_.GetCustomAttribute<TestInfo>().Unity.GetDescription();

                    List<KeyValuePair<string, double>> dynamic_points = new List<KeyValuePair<string, double>>();
                    foreach (var test in test_list_.OrderBy(x => x.Date))
                    {
                        dynamic_points.Add(new KeyValuePair<string, double>(
                            test.Date.ToShortDateString(), (double)indicator_.GetValue(test)
                            ));
                    }
                    linechartDynamics.ItemsSource = dynamic_points;
                    ((LinearAxis)chartControl.Resources["YAxis"]).Title = indicator_unity;
                    //axisX.Visibility = System.Windows.Visibility.Visible;
                    //axisY.Visibility = System.Windows.Visibility.Visible;
                    //axisX.ShowGridLines = true;
                    //axisY.ShowGridLines = true;
                    //axisY.Title = indicator_.GetCustomAttribute<TestInfo>().Unity.GetDescription();
                    maxnormline.ItemsSource = null;
                    minnormline.ItemsSource = null;
                    double max = (from x in dynamic_points select x.Value).Max();
                    double min = (from x in dynamic_points select x.Value).Min();
                    double min_norm = PatientDB.Instance.GetTestNorms(test_type)[indicator_.Name].Min;
                    double max_norm = PatientDB.Instance.GetTestNorms(test_type)[indicator_.Name].Max;
                    if (max >= max_norm) 
                    {
                        maxnormline.ItemsSource = (from norm in dynamic_points select new KeyValuePair<string, double>(norm.Key, max_norm)).ToList();
                    }
                    if (min <= min_norm)
                    {
                        minnormline.ItemsSource = (from norm in dynamic_points select new KeyValuePair<string, double>(norm.Key, min_norm)).ToList();
                    }
                    chartControl.FontSize = 14;
                    chartControl.Width = 100 * dynamic_points.Count + 210;
                    canvasGraph.Width = chartControl.Width;
                    canvasGraph.Height = chartControl.Height;

                }
            }
            catch (Exception exception)
            {
                string message = String.Format("Обнаружена проблема при работе программы!\n{0}", exception.Message);
                MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //public string DynamicLineUnity { get { return indicator_unity; } }
    }
}
