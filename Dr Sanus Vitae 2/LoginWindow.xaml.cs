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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void linkSignUp_Click(object sender, RoutedEventArgs e)
        {
            new SignUpWindow().ShowDialog();
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(textboxLogin.Text) && !String.IsNullOrWhiteSpace(textboxPassword.Password))
                {
                    PersonalInfo personal_info;
                    if ((personal_info = SettingsManager.Instance.PersonalInfo) != null)
                    {
                        if (personal_info.Login == textboxLogin.Text && textboxPassword.Password.StrEqualsMD5Hash(personal_info.Password))
                        {
                            new MainWindow().Show();
                            Close();
                        }
                        else MessageBox.Show("Поля логина или пароля заполнены не правильно!", "Ошибка входа!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else MessageBox.Show("Поля логина или пароля заполнены не правильно!", "Ошибка входа!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    MessageBox.Show("Не все поля заполнены!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception exception)
            {
                MessageBox.Show(String.Format("Обнаружена ошибка при работе с программой!", exception.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void linkRemindPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PersonalInfo personal_info = SettingsManager.Instance.PersonalInfo;
                if (personal_info != null || personal_info.UseSecretQuestion)
                {
                    if (new InputBox().RunDialog<bool>(personal_info.SecretQuestion, x => x.Trim(' ').StrEqualsMD5Hash(personal_info.Answer)))
                    {
                        var dialog_result = new PasswordUpdate().UpdatePassword();
                        if (dialog_result.Item2)
                        {
                            personal_info.Password = dialog_result.Item1.StrToMD5Hash();
                            SettingsManager.Instance.PersonalInfo = personal_info;
                        }
                    }
                    else
                        MessageBox.Show("Вы неправильно ответили на секретный вопрос! Попытайтесь еще раз.", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                    MessageBox.Show("Вы не зарегистрированы!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch(Exception exception)
            {
                MessageBox.Show(String.Format("Обнаружена ошибка при работе с программой!", exception.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
