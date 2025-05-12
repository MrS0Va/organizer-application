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
using System.Windows.Shapes;

namespace organizer_application
{
    public partial class EditTaskWindow : Window
    {
        public event EventHandler<TaskModel> TaskEdited;
        private TaskModel _task;

        public EditTaskWindow(TaskModel task)
        {
            InitializeComponent();
            _task = task;
            LoadTaskDetails();
        }

        private void LoadTaskDetails()
        {
            TitleTextBox.Text = _task.Title;
            DescriptionTextBox.Text = _task.Description;
            DueDatePicker.SelectedDate = _task.DueDate;
            PriorityComboBox.SelectedItem = PriorityComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == _task.Priority);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _task.Title = TitleTextBox.Text;
            _task.Description = DescriptionTextBox.Text;
            _task.DueDate = DueDatePicker.SelectedDate.Value;
            _task.Priority = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Код для обновления задачи в базе данных

            TaskEdited?.Invoke(this, _task);
            Close();
        }
    }
}
