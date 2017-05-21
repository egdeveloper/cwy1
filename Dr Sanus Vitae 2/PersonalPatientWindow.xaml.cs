using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Win32;
using System.Windows.Controls.DataVisualization.Charting;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using Novacode;

namespace SanusVitae
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return en.ToString();
        }

    }
    /// <summary>
    /// Interaction logic for ПрофильПациентаОкно.xaml
    /// </summary>
    public partial class PersonalPatientWindow : Window
    {
        private IEnumerable<MedicalTest> test_list_;
        private Type test_type;
        private IEnumerable<PropertyInfo> properties_;
        private PropertyInfo indicator_;
        private TestNorms norms_;
        private ObservableCollection<Attachment> attachments_;
        public Patient CurrentPatient { get; set; }
        public PersonalPatientWindow()
        {
            InitializeComponent();
            attachments_ = new ObservableCollection<Attachment>();
            listboxAttachments.ItemsSource = attachments_;
            //PatientDB.Instance.DBUpdated += comboboxIndexType_Selected;
        }
        private void linkUpdatePatientPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Filter = "Изображения (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            if (openfileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openfileDialog.FileName));
                pictureboxPatientPhoto.Source = bitmap;
                if (CurrentPatient == null)
                    return;
                CurrentPatient.Photo = File.ReadAllBytes(openfileDialog.FileName);
            }     
        }

        private void buttonUpdatePatientRecord_Click(object sender, RoutedEventArgs e)
        {
            Patient patient = (Patient)dialogPatientPage.DataContext;
            patient.Sex = (Sex)comboboxSex.SelectedIndex;
            patient.BloodGroup = (BloodGroup)comboboxBloodGroup.SelectedIndex;
            patient.Rh = (Rh)comboboxRh.SelectedIndex;
            patient.Disability = (Disability)comboboxDisability.SelectedIndex;
            PatientDB.Instance.AddPatient(ref patient);
            CurrentPatient = patient;
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            foreach(var child in dialogPatientPage.Children.OfType<TextBox>()) //очищаем все поля заполения
                child.Clear();
            foreach (var child in dialogPatientPage.Children.OfType<ComboBox>())  //удаляем все выбранные элементы выпадающих списков
                child.SelectedItem = null;
            pictureboxPatientPhoto.Source = null; //удаляем картинку пациента

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dialogPatientPage.DataContext = CurrentPatient;
                comboboxSex.SelectedIndex = (int)CurrentPatient.Sex;
                comboboxRh.SelectedIndex = (int)CurrentPatient.Rh;
                comboboxBloodGroup.SelectedIndex = (int)CurrentPatient.BloodGroup;
                comboboxDisability.SelectedIndex = (int)CurrentPatient.Disability;
                if (CurrentPatient.Photo == null)
                    return;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(CurrentPatient.Photo);
                bitmap.EndInit();
                pictureboxPatientPhoto.Source = bitmap;
            }
            catch(Exception exception)
            {
                MessageBox.Show(String.Format("Обнаружена ошибка при работе программы!\n{0}", exception.Message), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void comboboxTestType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combox_control = sender as ComboBox;
            if (combox_control != null)
            {
                if (combox_control == comboboxTestType)
                {
                    containerTestControl.Children.Clear();
                    buttonAddTest.IsEnabled = true;
                    switch (combox_control.SelectedIndex)
                    {
                        case 0:
                            containerTestControl.Children.Add(new CommonBloodTestPage());
                            test_type = typeof(CommonBloodTest);
                            break;
                        case 1:
                            containerTestControl.Children.Add(new BiochemBloodTestPage());
                            test_type = typeof(BioChemTest);
                            break;
                        case 2:
                            containerTestControl.Children.Add(new DailyExcreationTestPage());
                            test_type = typeof(DailyExcreationTest);
                            break;
                        case 3:
                            containerTestControl.Children.Add(new ColorTestPage());
                            test_type = typeof(UreaColorTest);
                            break;
                        case 4:
                            containerTestControl.Children.Add(new CommonUreaTestPage());
                            test_type = typeof(CommonUreaTest);
                            break;
                        case 5:
                            containerTestControl.Children.Add(new TitrationTestPage());
                            test_type = typeof(TitrationTest);
                            break;
                        case 6:
                            containerTestControl.Children.Add(new UreaStoneTestPage());
                            test_type = typeof(UreaStoneTest);
                            break;
                    }
                }
            }
        }

        private void buttonUpdateTestRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (datepickerTestDate.SelectedDate == null)
                {
                    MessageBox.Show("Вы не ввели дату сдачи анализа!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                MedicalTest test = ((IDataContextProvider<MedicalTest>)containerTestControl.Children[0]).ProvideDataContext();
                test.PatientID = CurrentPatient.ID;
                test.Treatment = textboxTreatment.Text;
                test.Date = datepickerTestDate.SelectedDate ?? DateTime.Now.Date;
                foreach(var attachment in attachments_){
                    attachment.TableOwner = test.GetType().FullName;
                    attachment.TestID = test.ID;
                }
                test.Attachments = attachments_.ToList();
                PatientDB.Instance.AddAnalysis(ref test, test_type);
                MessageBox.Show("Результаты анализов сохранены!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(String.Format("Обнаружена ошибка при работе программы!\n{0}", exception.Message), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void pictureboxPatientPhoto_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string file_ = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                pictureboxPatientPhoto.Source = new BitmapImage(new Uri(file_));
            }
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
            SaveFileDialog save_fileDialog = new SaveFileDialog() { Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp|Word документ (*.docx)|*.docx" };
            if (save_fileDialog.ShowDialog() == true && !String.IsNullOrEmpty(save_fileDialog.FileName))
            {
                norms_ = PatientDB.Instance.GetTestNorms(test_type)[indicator_.Name];
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

        private void contextMenuItemCopyIntoClipboard_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)chartControl.ActualWidth, (int)chartControl.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(chartControl);
            Clipboard.SetImage(rtb);
        }
        private void AddResourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void buttonAddAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (test_type == null)
            {
                MessageBox.Show("Вы не выбрали тип анализа!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            OpenFileDialog open_filedialog = new OpenFileDialog() {Title = "Добавить прикрепляемый файл к анализу"};
            if(open_filedialog.ShowDialog() == true){
                string filename = open_filedialog.FileName.Substring(open_filedialog.FileName.LastIndexOf(@"\") + 1);
                attachments_.Add(new Attachment()
                {
                    Name = filename,
                    Item = File.ReadAllBytes(open_filedialog.FileName),
                    Extension = filename.Substring(filename.LastIndexOf('.') + 1)
                });
            }
        }

        private void buttonRemoveAttachment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listboxAttachments.SelectedItem == null)
                {
                    MessageBox.Show("Вы не выбрали прикрепляемый файл, который нужно удалить!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                attachments_.Remove((Attachment)listboxAttachments.SelectedItem);
            }
            catch(Exception exception)
            {
                MessageBox.Show(String.Format("Обнаружена ошибка при работе программы!\n{0}", exception.Message), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonPlotGraph_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                if (comboboxIndexType.SelectedIndex >= 0)
                {
                    if (test_list_.Count() <= 0)
                        return;
                    indicator_ = properties_.ToArray()[comboboxIndexType.SelectedIndex];

                    chartControl.Title = indicator_.GetCustomAttribute<TestInfo>().DisplayName;

                    List<KeyValuePair<string, double>> dynamic_points = new List<KeyValuePair<string, double>>();
                    foreach (var test in test_list_.OrderBy(x => x.Date))
                    {
                        dynamic_points.Add(new KeyValuePair<string, double>(
                             test.Date.ToShortDateString(), (double)indicator_.GetValue(test)));
                    }
                    //LineSeries linechartDynamics = new LineSeries();
                    linechartDynamics.ItemsSource = dynamic_points;
                    //linechartDynamics.IndependentAxis.Orientation = AxisOrientation.X;
                    axisY.Visibility = System.Windows.Visibility.Visible;
                    axisY.ShowGridLines = true;
                    axisY.Title = indicator_.GetCustomAttribute<TestInfo>().Unity.GetDescription();
                    double min = dynamic_points.Min(x => x.Value);
                    double max = dynamic_points.Max(x => x.Value);
                    chartControl.Width = 100 * dynamic_points.Count + 210;
                    canvasGraph.Width = chartControl.Width;
                    canvasGraph.Height = chartControl.Height;
                    
                    //TestNorms norms = PatientDB.Instance.GetTestNorms(test_type)[indicator_.Name];
                    //List<KeyValuePair<string, double>> max_norm = (from date in dynamic_points select new KeyValuePair<string, double>(date.Key, norms.Max)).ToList();
                    //List<KeyValuePair<string, double>> min_norm = (from date in dynamic_points select new KeyValuePair<string, double>(date.Key, norms.Min)).ToList();
                    //LineSeries max_line = new LineSeries()
                    //{
                      //  ItemsSource = max_norm,
                        //DependentRangeAxis = axisY
                    //};
                    //LineSeries min_line = new LineSeries()
                    //{
                      //  ItemsSource = min_norm,
                        //DependentRangeAxis = axisY
                    //};
                    //chartControl.Series.Add(linechartDynamics);
                    //chartControl.Series.Add(max_line);
                    //chartControl.Series.Add(min_line);
                }
            //}
            //catch (Exception exception)
            //{
              //  MessageBox.Show(String.Format("Обнаружена ошибка при работе программы!\n{0}", exception.Message), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
    }
}
