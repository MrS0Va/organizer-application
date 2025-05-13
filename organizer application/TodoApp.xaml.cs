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
using System.Windows.Shapes;

namespace organizer_application
{
    public partial class TodoApp : Window
    {
        private ThemeViewModel _themeViewModel;
        public TodoApp()
        {
            InitializeComponent();

            _themeViewModel = new ThemeViewModel();
            DataContext = _themeViewModel;

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            LoadTasks();
        }

        private void ThemeManager_ThemeChanged(object sender, EventArgs e)
        {
        }

        private void LoadTasks()
        {
            TasksListBox.Items.Clear();

            try
            {
                using (SqlConnection connection = DataBaseConnect.GetConnection())
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM Tasks ORDER BY DueDate ASC", connection);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TaskModel task = new TaskModel
                            {
                                Id = (int)reader["Id"],
                                Title = reader["Title"] as string,
                                Description = reader["Description"] as string,
                                DueDate = (DateTime)reader["DueDate"],
                                Priority = reader["Priority"] as string,
                                Category = reader["Category"] as string,
                                IsCompleted = (bool)reader["IsCompleted"]
                            };
                            TasksListBox.Items.Add(task);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке задач: " + ex.Message);
            }
        }

        // Открываем окно добавления задачи
        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddTaskWindow();
            addWindow.TaskAdded += (s, eArgs) => LoadTasks();
            addWindow.ShowDialog();
        }

        // Открываем окно редактирования выбранной задачи
        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListBox.SelectedItem is TaskModel selectedTask)
            {
                var editWindow = new EditTaskWindow(selectedTask);
                editWindow.TaskEdited += (s, eArgs) => LoadTasks();
                editWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите задачу для редактирования.");
            }
        }

        // Удаление выбранной задачи
        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListBox.SelectedItem is TaskModel selectedTask)
            {
                var result = MessageBox.Show($"Удалить задачу: \"{selectedTask.Title}\"?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (SqlConnection connection = DataBaseConnect.GetConnection())
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand("DELETE FROM Tasks WHERE Id = @Id", connection);
                            command.Parameters.AddWithValue("@Id", selectedTask.Id);
                            command.ExecuteNonQuery();
                        }
                        LoadTasks();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении задачи: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите задачу для удаления.");
            }
        }
    }
}
