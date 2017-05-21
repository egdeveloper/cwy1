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

namespace SanusVitae
{
    /// <summary>
    /// Interaction logic for РегистрацияОкно.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        private PersonalInfo personal_info;
        public SignUpWindow()
        {
            InitializeComponent();
            personal_info = new PersonalInfo();
            this.gridSignUpDialog.DataContext = personal_info;
        }

        private void buttonSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (checkboxUseSecretQuestion.IsChecked.Value && !String.IsNullOrWhiteSpace(textboxSecretQuestion.Text) && !String.IsNullOrWhiteSpace(textboxAnswer.Text))
            {
                if (gridSignUpDialog.Children.OfType<TextBox>().All(x => !String.IsNullOrWhiteSpace(x.Text)) && gridSignUpDialog.Children.OfType<PasswordBox>().All(x => !String.IsNullOrWhiteSpace(x.Password)))
                {
                    if (textboxPassword.Password != textboxPasswordCheck.Password)
                        MessageBox.Show("Подтвердите пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        personal_info.Password = textboxPassword.Password.StrToMD5Hash();
                        personal_info.UseSecretQuestion = checkboxUseSecretQuestion.IsChecked.Value;
                        personal_info.Answer = personal_info.Answer.StrToMD5Hash();
                        SettingsManager.Instance.PersonalInfo = personal_info;
                        Close();
                    }
                }
                else 
                    MessageBox.Show("Не все поля заполнены!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else 
                MessageBox.Show("Не все поля заполнены!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
