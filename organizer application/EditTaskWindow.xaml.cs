using organizer_application.Models;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace organizer_application
{
    public partial class EditTaskWindow : Window
    {
        public event EventHandler TaskEdited;
        private TaskModel _task;

        public EditTaskWindow(TaskModel task)
        {
            InitializeComponent();
            _task = task;

            // Заполнение полей данными задачи
            TitleTextBox.Text = _task.Title;
            DescriptionTextBox.Text = _task.Description;
            DueDatePicker.SelectedDate = _task.DueDate;
            PriorityComboBox.SelectedItem = PriorityComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == _task.Priority);
            StatusComboBox.SelectedItem = StatusComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == _task.Status); // Предполагается, что у TaskModel есть свойство Status
        }

        private void SaveAndChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || DueDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните обязательные поля.");
                return;
            }

            // Обновление данных задачи
            _task.Title = TitleTextBox.Text;
            _task.Description = DescriptionTextBox.Text;
            _task.DueDate = DueDatePicker.SelectedDate.Value;
            _task.Priority = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _task.Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            try
            {
                using (SqlConnection connection = DataBaseConnect.GetConnection())
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        "UPDATE Tasks SET Title = @Title, Description = @Description, DueDate = @DueDate, Priority = @Priority, Status = @Status WHERE Id = @Id",
                        connection);
                    command.Parameters.AddWithValue("@Title", _task.Title);
                    command.Parameters.AddWithValue("@Description", (object)_task.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DueDate", _task.DueDate);
                    command.Parameters.AddWithValue("@Priority", _task.Priority);
                    command.Parameters.AddWithValue("@Status", _task.Status);
                    command.Parameters.AddWithValue("@Id", _task.Id);
                    command.ExecuteNonQuery();
                }

                TaskEdited?.Invoke(this, EventArgs.Empty);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при редактировании задачи: " + ex.Message);
            }
        }
    }
}