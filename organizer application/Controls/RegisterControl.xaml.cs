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

            // Проверка, что логин не пустой
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Пожалуйста, введите логин.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка, что пароль не пустой
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Проверка на существование пользователя с таким логином
            var existingUser = DataBaseConnect.GetUserByUsername(username);
            if (existingUser != null)
            {
                MessageBox.Show("Этот логин уже занят. Пожалуйста, выберите другой.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка минимальной длины пароля
            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен быть не менее 6 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Попытка зарегистрировать пользователя
            try
            {
                DataBaseConnect.RegisterUser(username, password);
                MessageBox.Show("Регистрация прошла успешно! Пожалуйста, войдите в систему.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                // Переключение на окно входа или вызов метода для загрузки окна авторизации
                ((MainWindow)Window.GetWindow(this)).LoadLoginControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoginTextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).LoadLoginControl();
        }
    }
}
