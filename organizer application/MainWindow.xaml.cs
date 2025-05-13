using organizer_application.Controls;
using organizer_application.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace organizer_application
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadLoginControl();
        }
        public void LoadLoginControl()
        {
            MainContent.Content = new LoginControl();
        }
        public void LoadRegisterControl()
        {
            MainContent.Content = new RegisterControl();
        }
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadLoginControl();
        }
        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadRegisterControl();
        }
    }
}
