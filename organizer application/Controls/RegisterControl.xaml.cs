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

namespace organizer_application.Controls
{
    public partial class RegisterControl : UserControl
    {
        public RegisterControl()
        {
            InitializeComponent();
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = NewUsernameTextBox.Text;
            string password = NewPasswordBox.Password;
            try
            {
                DataBaseConnect.RegisterUser(username, password);
                MessageBox.Show("Registration successful! Please log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // Переключаем на вкладку авторизации
                ((MainWindow)Window.GetWindow(this)).LoadLoginControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoginTextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).LoadLoginControl();
        }
    }
}
