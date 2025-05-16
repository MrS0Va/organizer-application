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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Просто закрывает окно
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || DueDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните обязательные поля.");
                return;
            }

            TimeSpan reminderOffset = TimeSpan.FromMinutes(10); // по умолчанию
            if (ReminderComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                if (TimeSpan.TryParse(selectedItem.Tag.ToString(), out TimeSpan parsed))
                {
                    reminderOffset = parsed;
                }
            }


            TaskModel newTask = new TaskModel
            {
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text,
                DueDate = DueDatePicker.SelectedDate.Value,
                Priority = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Category = null, // при необходимости добавьте
                ReminderOffset = reminderOffset
            };

            try
            {
                using (SqlConnection connection = DataBaseConnect.GetConnection())
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Tasks (Title, Description, DueDate, Priority, Category, UserId, Status) VALUES (@Title, @Description, @DueDate, @Priority, @Category, @UserId, @Status)",
                        connection);
                    cmd.Parameters.AddWithValue("@Title", newTask.Title);
                    cmd.Parameters.AddWithValue("@Description", (object)newTask.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DueDate", newTask.DueDate);
                    cmd.Parameters.AddWithValue("@Priority", newTask.Priority);
                    cmd.Parameters.AddWithValue("@Category", (object)newTask.Category ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", Session.CurrentUser.Id);
                    cmd.Parameters.AddWithValue("@Status", newTask.Status ?? "Невыполнено");
                    cmd.ExecuteNonQuery();
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
