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
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        public InputBox()
        {
            InitializeComponent();
        }
        public T RunDialog<T>(string message, Func<string, T> action)
        {
            labelMessage.Content = message;
            ShowDialog();
            return action(textboxAnswer.Text);
        }
        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
