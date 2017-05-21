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
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;

namespace SanusVitae
{
    /// <summary>
    /// Interaction logic for TestHistory.xaml
    /// </summary>
    public partial class TestHistoryWindow : Window
    {
        private bool searching_ = false;
        private class ProcedureInfo
        {
            public Patient Patient { get; set; }
            public MedicalTest Test { get; set; }
        }
        private List<ProcedureInfo> test_info_list;
        private readonly Type[] test_types = { 
        typeof(CommonBloodTest), typeof(BioChemTest), typeof(CommonUreaTest),
        typeof(TitrationTest), typeof(DailyExcreationTest), typeof(UreaColorTest), typeof(UreaStoneTest)  
        };                                      
        public TestHistoryWindow()
        {
            InitializeComponent();
            test_info_list = new List<ProcedureInfo>();
            PatientDB.Instance.DBUpdated += datagridTestHistoryDataUpdated;
        }
        private void menuitemSaveTestAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (datagridProcedureInfo.SelectedItem != null)
                {
                    ProcedureInfo selected_test_info = (ProcedureInfo)datagridProcedureInfo.SelectedItem;
                    SaveFileDialog save_file_dialog = new SaveFileDialog() { Filter = "Word документ (*.docx)|*.docx|Zip архив (*.zip)|*.zip" };
                    if (save_file_dialog.ShowDialog() == true)
                    {
                        string filename = save_file_dialog.FileName;
                        switch (filename.Substring(filename.LastIndexOf('.') + 1))
                        {
                            case "docx":
                                DocumentGenerator.CreateTestReport(selected_test_info.Patient, selected_test_info.Test, filename);
                                break;
                            case "zip":
                                string directorypath = System.IO.Path.GetDirectoryName(filename);
                                filename = filename.Substring(filename.LastIndexOf(@"\") + 1, filename.LastIndexOf(".") - filename.LastIndexOf(@"\") - 1);
                                Directory.CreateDirectory(directorypath + @"\" + filename);
                                DocumentGenerator.CreateTestReport(selected_test_info.Patient, selected_test_info.Test, directorypath + @"\" + filename + @"\" + filename + ".docx");
                                foreach (var attachment in selected_test_info.Test.Attachments)
                                {
                                    File.WriteAllBytes(directorypath + @"\" + filename + @"\" + attachment.Name, attachment.Item);
                                }
                                ZipFile.CreateFromDirectory(directorypath + @"\" + filename, directorypath + @"\" + filename + ".zip");
                                Directory.Delete(directorypath + @"\" + filename, true);
                                break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                string message = String.Format("Обнаружена проблема при работе программы!\n{0}", exception.Message);
                MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }  
        }
        private void datagridTestHistoryDataUpdated(object sender, RoutedEventArgs e)
        {
            test_info_list.Clear();
            if (CurrentPatient == null)
            {
                foreach (var patient in PatientDB.Instance.GetPatients())
                {
                    foreach (var test_type in test_types)
                        foreach (var test in PatientDB.Instance.GetAnalysis(patient.ID, test_type))
                            test_info_list.Add(new ProcedureInfo()
                            {
                                Patient = patient, Test = test
                            });
                }
            }
            else
                foreach (var test_type in test_types)
                    foreach (var test in PatientDB.Instance.GetAnalysis(CurrentPatient.ID, test_type))
                        test_info_list.Add(new ProcedureInfo()
                        {
                            Patient = CurrentPatient,
                            Test = test
                        });
            datagridProcedureInfo.ItemsSource = test_info_list.OrderBy(x => x.Test.Date).ToList();
        }
        public Patient CurrentPatient { get; set; }

        private void buttonSearchTest_Click(object sender, RoutedEventArgs e)
        {
            if (!searching_)
            {
                if (flagName.IsChecked == true)
                {
                    datagridProcedureInfo.ItemsSource = test_info_list.Where(x => x.Patient.ToString().Contains(textboxSearchFilter.Text)).ToList();
                }
                else if (flagCardNumber.IsChecked == true)
                {
                    datagridProcedureInfo.ItemsSource = test_info_list.Where(x => x.Patient.CardNumber.Contains(textboxSearchFilter.Text)).ToList();
                }
                else if (flagTestType.IsChecked == true)
                {
                    datagridProcedureInfo.ItemsSource = test_info_list.Where(x => x.Test.ToString().Contains(textboxSearchFilter.Text)).ToList();
                }
                else if (flagDate.IsChecked == true)
                {
                    datagridProcedureInfo.ItemsSource = test_info_list.Where(x => x.Test.Date.ToString().Contains(textboxSearchFilter.Text)).ToList();
                }
                buttonSearchTest.Content = Resources["ClearIcon"];
                searching_ = true;
            }
            else
            {
                textboxSearchFilter.Text = String.Empty;
                datagridProcedureInfo.ItemsSource = test_info_list.OrderBy(x => x.Test.Date).ToList();
                searching_ = false;
                buttonSearchTest.Content = Resources["SearchIcon"];
            }
        }
    }

}
