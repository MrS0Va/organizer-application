using organizer_application.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using organizer_application.Models;
using System.Threading;
using System.Data.SqlClient;

namespace organizer_application
{
    
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThemeManager.CurrentThemeUri = ThemeManager.DarkThemeUri; //начальная тема
        }

        private void LoadAndScheduleReminders()
        {
            var tasks = LoadTasksFromDatabase(); // реализация чтения задач
            foreach (var task in tasks)
            {
                if (!task.IsCompleted)
                {
                    ReminderService.ScheduleReminder(task);
                }
            }
        }
        private static List<System.Threading.Timer> timers = new List<System.Threading.Timer>();

        public static void ScheduleReminder(TaskModel task)
        {
            DateTime reminderTime = task.DueDate - task.ReminderOffset;
            TimeSpan delay = reminderTime - DateTime.Now;

            if (delay <= TimeSpan.Zero)
            {
                // Время уже прошло, показываем уведомление сразу или пропускаем
                return;
            }

            var timer = new System.Threading.Timer((state) =>
            {
                ShowNotification(task);
            }, null, delay, Timeout.InfiniteTimeSpan);

            timers.Add(timer);
        }

        private static void ShowNotification(TaskModel task)
        {
            // Используем MessageBox
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"Напоминание: задача '{task.Title}' должна быть выполнена сегодня.", "Напоминание");
                
            });
        }

        private List<TaskModel> LoadTasksFromDatabase()
        {
            var tasks = new List<TaskModel>();

            using (SqlConnection connection = DataBaseConnect.GetConnection())
            {
                connection.Open();
                string query = "SELECT Id, Title, Description, DueDate, Priority, Category, Status, IsCompleted, ReminderTime FROM Tasks WHERE UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", Session.CurrentUser.Id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var task = new TaskModel
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                DueDate = reader.GetDateTime(3),
                                Priority = reader.GetString(4),
                                Category = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Status = reader.IsDBNull(6) ? null : reader.GetString(6),
                                IsCompleted = reader.GetBoolean(7),
                                ReminderOffset = reader.IsDBNull(8) ? TimeSpan.FromMinutes(10) : reader.GetTimeSpan(8) // убедитесь, что возвращается TimeSpan
                            };
                            tasks.Add(task);
                        }
                    }
                }
            }

            return tasks;
        }
    }
}
