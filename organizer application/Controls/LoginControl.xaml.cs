using organizer_application.Models;
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
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            var user = DataBaseConnect.GetUserByUsername(username);
            if (user == null)
            {
                MessageBox.Show("Пользователь с таким логином не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка правильности пароля
            if (DataBaseConnect.ValidateUser(username, password))
            {
                Session.CurrentUser = user;
                MessageBox.Show("Авторизация успешная! Добро пожаловать!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // Открываем TodoApp
                TodoApp todoApp = new TodoApp();
                todoApp.Show();
                Window.GetWindow(this).Close();
            }
            else
            {
                MessageBox.Show("Некорректный пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RegisterTextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).LoadRegisterControl();
        }
    }
}
