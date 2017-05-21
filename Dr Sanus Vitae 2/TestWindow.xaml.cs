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
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.Data.SQLite;
using Microsoft.Win32;

namespace SanusVitae
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private ObservableCollection<Attachment> attachments_;
        private Type test_type;
        public TestWindow()
        {
            InitializeComponent();
            attachments_ = new ObservableCollection<Attachment>();
            listboxAttachments.ItemsSource = attachments_;
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
                    Title = CurrentPatient.ToString() + " - " + test_type.GetCustomAttribute<DisplayNameAttribute>().DisplayName;
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
                test.Attachments = attachments_.ToList();
                PatientDB.Instance.AddAnalysis(ref test, test_type);
                MessageBox.Show("Результаты анализов сохранены!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(SQLiteException exception)
            {
                string message = String.Format("Обнаружена проблема при работе с базой данных!\n{0}", exception.Message);
                MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception exception)
            {
                string message = String.Format("Обнаружена проблема при работе программы!\n{0}", exception.Message);
                MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddResourceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        public Patient CurrentPatient { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = CurrentPatient.ToString();
        }
        private void buttonAddAttachment_Click(object sender, RoutedEventArgs e)
        {
            if (test_type == null)
            {
                MessageBox.Show("Вы не выбрали тип анализа!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            OpenFileDialog open_filedialog = new OpenFileDialog() { Title = "Добавить прикрепляемый файл к анализу" };
            if (open_filedialog.ShowDialog() == true)
            {
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
            if (listboxAttachments.SelectedItem == null)
            {
                MessageBox.Show("Вы не выбрали прикрепляемый файл, который нужно удалить!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            attachments_.Remove((Attachment)listboxAttachments.SelectedItem);
        }
    }
}
