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
using System.Windows.Controls.Ribbon;
using System.IO;
using Microsoft.Win32;
using Novacode;
using System.ComponentModel;
using System.Reflection;
namespace SanusVitae
{
    /// <summary>
    /// Interaction logic for ГлавноеОкно.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private List<Patient> patients;
        private List<Patient> filtered_patients_list;
        private Patient selected_patient;
        private bool show_fpl = false;
        public MainWindow()
        {
            InitializeComponent();
            datagridPatientList.DataContext = new Patient();
            try
            {
                patients = PatientDB.Instance.GetPatients();
                PatientDB.Instance.DBUpdated += this.datagridPatientList_Updated;
            }
            catch (Exception exp)
            {
                MessageBox.Show("OOPS! Программа выбросила исключение: "  + exp.Message);
            }
        }

        private void buttonAboutApplication_Click(object sender, RoutedEventArgs e)
        {
            new AboutApplicationWindow().ShowDialog();
        }
        private void buttonAddPatient_Click(object sender, RoutedEventArgs e)
        {
            new PersonalPatientWindow() { CurrentPatient = new Patient() { Photo = default(byte[])} }.ShowDialog();
        }
        private void datagridPatientList_Updated(object sender, RoutedEventArgs e)
        {
            try
            {
                patients = PatientDB.Instance.GetPatients();
                datagridPatientList.ItemsSource = patients;
                show_fpl = false;
            }
            catch(System.Data.SQLite.SQLiteException exception)
            {
                string message = "Обнаружена ошибка при работе с базой данных!";
                switch (exception.ErrorCode)
                {
                    case (int)System.Data.SQLite.SQLiteErrorCode.Corrupt:
                        message = String.Format("{0}\nФайл базы данных испорчен и не может быть открыт!", message);
                        break;
                    case (int)System.Data.SQLite.SQLiteErrorCode.Auth:
                        message = String.Format("{0}\nУ Вас нет прав доступа к базе данных!", message);
                        break;
                    case (int)System.Data.SQLite.SQLiteErrorCode.Busy:
                        message = String.Format("{0}\nФайл базы данных занят другим процессом!", message);
                        break;
                    case (int)System.Data.SQLite.SQLiteErrorCode.Empty:
                        message = String.Format("{0}\nФайл базы данных пуст!");
                        break;
                }
                
                MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(Exception exception){
                string message = String.Format("Обнаружена ошибка при работе программы!\n{0}", exception.Message);
                MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void datagridPatientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((selected_patient = datagridPatientList.SelectedItem as Patient) != null)
            {
                
                if (!show_fpl)
                    selected_patient = patients[datagridPatientList.SelectedIndex];
                else
                    selected_patient = filtered_patients_list[datagridPatientList.SelectedIndex];
                ЛичныеДанныеПациента.DataContext = selected_patient;
                МужскойКнопка.IsEnabled = selected_patient.Sex == Sex.мужской;
                МужскойКнопка.IsChecked = selected_patient.Sex == Sex.мужской;
                ЖенскийКнопка.IsEnabled = МужскойКнопка.IsEnabled ^ true;
                ЖенскийКнопка.IsChecked = МужскойКнопка.IsChecked ^ true;
                buttonOpenPatientProfile.IsEnabled = true;
                if (selected_patient.Photo == null)
                {
                    pictureboxPatientPhoto.Source = null;
                    return;
                }
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(selected_patient.Photo);
                bitmap.EndInit();
                pictureboxPatientPhoto.Source = bitmap;
            }
        }

        private void buttonSearchPatients_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!show_fpl)
                {
                    if (flagName.IsChecked == true)
                        filtered_patients_list = patients.Where(x => (x.LastName + " " + x.FirstName + " " + x.Patronym).IndexOf(textboxSearchFilter.Text.Trim(' '), StringComparison.CurrentCultureIgnoreCase) > -1).ToList();
                    else if (flagCardNumber.IsChecked == true)
                        filtered_patients_list = patients.Where(x => x.CardNumber.IndexOf(textboxSearchFilter.Text.Trim(' '), StringComparison.CurrentCultureIgnoreCase) > -1).ToList();
                    else
                        return;
                    if (filtered_patients_list.Count <= 0)
                    {
                        MessageBox.Show("Пациента с таким именем нет!");
                        return;
                    }
                    datagridPatientList.ItemsSource = filtered_patients_list;
                    show_fpl = true;
                    ((Button)sender).Content = Resources["ClearIcon"];
                }
                else
                {
                    ((Button)sender).Content = Resources["SearchIcon"];
                    show_fpl = false;
                    datagridPatientList_Updated(null, null);
                    textboxSearchFilter.Text = String.Empty;
                }
            }
            catch(Exception exception)
            {
                string message = String.Format("Обнаружена ошибка при работе программы!\n{0}", exception.Message);
                MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DBCommands_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is DataGrid)
            {
                if (e.Command == (RoutedUICommand)this.Resources["DBInsert"])
                    e.CanExecute = true;
                e.CanExecute = ((DataGrid)sender).SelectedIndex != -1;
            }
        }
        private void OpenPatient_Command_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is DataGrid)
            {
                new PersonalPatientWindow() { CurrentPatient = (Patient)((DataGrid)sender).SelectedItem}.ShowDialog();
            }
        }
        private void DBInsert_Command_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new PersonalPatientWindow() { CurrentPatient = new Patient() { Photo = default(byte[]) } }.ShowDialog();
        }
        private void RemovePatient_Command_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (sender is DataGrid)
                {
                    if (MessageBox.Show("Вы действительно хотите удалить записи пациента", "Удаление записей пациента", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Patient patient = (Patient)((DataGrid)sender).SelectedItem;
                        patients.Remove(patient);
                        ((DataGrid)sender).ItemsSource = patients;
                        PatientDB.Instance.RemovePatient(patient.ID);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddTest_Command_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is DataGrid)
            {
                new TestWindow() { CurrentPatient = (Patient)((DataGrid)sender).SelectedItem }.Show();
            }
        }
        private void PlotGraph_Command_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is DataGrid)
            {
                new DynamicsWindow() { CurrentPatient = (Patient)((DataGrid)sender).SelectedItem }.Show();
            }
        }
        private void buttonShowTestHistory_Click(object sender, RoutedEventArgs e)
        {
            new TestHistoryWindow() { CurrentPatient = (Patient)datagridPatientList.SelectedItem }.Show();
        }

        private void buttonDBConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog open_filedialog = new OpenFileDialog() { Filter = "Файл БД (*.sqlite)|*.sqlite" };
                if (open_filedialog.ShowDialog() == true)
                    PatientDB.Instance.ConnectToDB(open_filedialog.FileName);
            }
            catch(Exception exp){
                MessageBox.Show(exp.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void buttonDBSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog save_filedialog = new SaveFileDialog() { Filter = "Файл БД (*.sqlite)|*.sqlite" };
                if (save_filedialog.ShowDialog() == true)
                    PatientDB.Instance.SaveDBLocally(save_filedialog.FileName);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Вы действительно хотите завершить приложение?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes){
                Application.Current.Shutdown();
            }
        }

        private void buttonGetHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.Help.ShowHelp(null, "Help.chm");
            }
            catch(Exception)
            {
                MessageBox.Show("При получении справки произошел сбой работы программы!");
            }
        }
    }
}
