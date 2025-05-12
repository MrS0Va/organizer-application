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
    public partial class AddTaskWindow : Window
    {
        public event EventHandler<TaskModel> TaskAdded;

        public AddTaskWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || DueDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните обязательные поля.");
                return;
            }

            TaskModel newTask = new TaskModel
            {
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text,
                DueDate = DueDatePicker.SelectedDate.Value,
                Priority = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Category = null // Если у вас есть поле для категории, добавьте его
            };

            try
            {
                using (SqlConnection connection = DataBaseConnect.GetConnection())
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        "INSERT INTO Tasks (Title, Description, DueDate, Priority, Category) VALUES (@Title, @Description, @DueDate, @Priority, @Category)",
                        connection);
                    command.Parameters.AddWithValue("@Title", newTask.Title);
                    command.Parameters.AddWithValue("@Description", (object)newTask.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DueDate", newTask.DueDate);
                    command.Parameters.AddWithValue("@Priority", newTask.Priority);
                    command.Parameters.AddWithValue("@Category", (object)newTask.Category ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }

                TaskAdded?.Invoke(this, newTask);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении задачи: " + ex.Message);
            }
        }
    }
}
