using organizer_application.Models;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

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
            LoadTasks("DueDate", true);
        }

        private void ThemeManager_ThemeChanged(object sender, EventArgs e)
        {
        }

        private bool _isSorting = false; // чтобы избежать рекурсии

        private void TasksDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (_isSorting)
                return;

            _isSorting = true;

            try
            {
                var column = e.Column;
                string sortBy = column.SortMemberPath;

                // Определяем направление сортировки
                ListSortDirection direction = (column.SortDirection != ListSortDirection.Ascending)
                    ? ListSortDirection.Ascending
                    : ListSortDirection.Descending;

                // Устанавливаем стрелку сортировки
                column.SortDirection = direction;

                // Сортируем коллекцию
                var items = TasksDataGrid.ItemsSource as System.ComponentModel.ICollectionView;

                if (items != null)
                {
                    items.SortDescriptions.Clear();
                    items.SortDescriptions.Add(new System.ComponentModel.SortDescription(sortBy, direction));
                }
                else
                {
                    // Если ItemsSource не установлен, сортируем Items
                    var list = TasksDataGrid.ItemsSource as System.Collections.IEnumerable;
                    
                    LoadTasks(sortBy, direction == ListSortDirection.Ascending);
                }
            }
            finally
            {
                _isSorting = false;
                e.Handled = true;
            }
        }

        private void LoadTasks(string sortBy = "DueDate", bool ascending = true)
        {
            TasksDataGrid.ItemsSource = null;

            try
            {
                using (SqlConnection connection = DataBaseConnect.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM Tasks WHERE UserId = @UserId";

                    if (sortBy == "Priority")
                    {
                        string priorityOrder = "CASE WHEN Priority = 'Высокий' THEN 1 WHEN Priority = 'Средний' THEN 2 WHEN Priority = 'Низкий' THEN 3 END";
                        query += $" ORDER BY {priorityOrder} {(ascending ? "ASC" : "DESC")}";
                    }
                    else // по дате
                    {
                        query += $" ORDER BY DueDate {(ascending ? "ASC" : "DESC")}";
                    }

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", Session.CurrentUser.Id);

                    var taskList = new System.Collections.ObjectModel.ObservableCollection<TaskModel>();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var task = new TaskModel
                            {
                                Id = (int)reader["Id"],
                                Title = reader["Title"] as string,
                                Description = reader["Description"] as string,
                                DueDate = (DateTime)reader["DueDate"],
                                Priority = reader["Priority"] as string,
                                Category = reader["Category"] as string,
                                Status = reader["Status"] as string,
                            };
                            taskList.Add(task);
                        }
                    }

                    // Устанавливаем в ItemsSource
                    TasksDataGrid.ItemsSource = taskList;

                    // Обеспечиваем сортировку по умолчанию
                    var view = System.Windows.Data.CollectionViewSource.GetDefaultView(TasksDataGrid.ItemsSource);
                    if (view != null && view.CanSort)
                    {
                        view.SortDescriptions.Clear();
                        view.SortDescriptions.Add(new System.ComponentModel.SortDescription(sortBy, ascending ? ListSortDirection.Ascending : ListSortDirection.Descending));
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
            if (TasksDataGrid.SelectedItem is TaskModel selectedTask) // Изменено на TasksDataGrid
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



        private void TasksDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column is DataGridCheckBoxColumn && e.Row.Item is TaskModel task)
            {
                // После редактирования чекбокса обновляем в базе
                Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        using (SqlConnection connection = DataBaseConnect.GetConnection())
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand("UPDATE Tasks SET IsCompleted = @IsCompleted WHERE Id = @Id", connection);
                            command.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);
                            command.Parameters.AddWithValue("@Id", task.Id);
                            command.ExecuteNonQuery();
                        }
                        // Обновляем отображение
                        LoadTasks();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при обновлении статуса: " + ex.Message);
                    }
                });
            }
        }

        //Выпонить задачу
        private void CompleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksDataGrid.SelectedItem is TaskModel selectedTask)
            {
                // Обновляем статус в базе данных
                try
                {
                    using (SqlConnection connection = DataBaseConnect.GetConnection())
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(
                            "UPDATE Tasks SET Status = @Status WHERE Id = @Id", connection);
                        command.Parameters.AddWithValue("@Status", "Выполнена");
                        command.Parameters.AddWithValue("@Id", selectedTask.Id);
                        command.ExecuteNonQuery();
                    }

                    // После успешного обновления в базе, обновляем свойство модели
                    selectedTask.Status = "Выполнено";
                    LoadTasks();


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении статуса: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите задачу для выполнения.");
            }
        }


        // Удаление выбранной задачи
        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksDataGrid.SelectedItem is TaskModel selectedTask) // Изменено на TasksDataGrid
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
