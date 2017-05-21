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
    /// Interaction logic for PasswordUpdate.xaml
    /// </summary>
    public partial class PasswordUpdate : Window
    {
        private bool result;
        public PasswordUpdate()
        {
            InitializeComponent();
        }
        public Tuple<string, bool> UpdatePassword()
        {
            ShowDialog();
            return new Tuple<string,bool>(textboxNewPassword.Password, result);
        }
        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (!textboxNewPassword.Password.Equals(textboxNewPasswordValidation.Password))
                MessageBox.Show("Подтвердите новый пароль!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                result = true;
                Close();
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            result = false;
            Close();
        }
    }
}
