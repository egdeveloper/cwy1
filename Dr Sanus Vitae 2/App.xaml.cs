using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Input;

namespace SanusVitae
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            try
            {
                SettingsManager.Instance.InitManager(Directory.GetCurrentDirectory() + @"\personal_settings.xml");
                PatientDB.Instance.InitDB("PatientDB.sqlite");
                this.Dispatcher.UnhandledException += (sender, e) =>
                {
                    using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + @"\error_logs.txt", true))
                    {
                        sw.WriteLine(DateTime.Now + ": " + e.Exception.Message + " " + e.Exception.StackTrace);
                    }
                };
            }
            catch (System.Data.SQLite.SQLiteException)
            {
                MessageBox.Show("Обнаружена ошибка при инициализации базы данных!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(Exception exception)
            {
                MessageBox.Show(String.Format("Обнаружена ошибка при работе программы!\n{0}", exception.Message), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
