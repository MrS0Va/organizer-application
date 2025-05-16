using organizer_application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace organizer_application.Service
{
    public static class ReminderService
    {
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
            // Используйте Windows Toast Notifications или MessageBox
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"Напоминание: задача '{task.Title}' должна быть выполнена сегодня.", "Напоминание");
                // Или реализуйте Toast Notification для Windows 10+
            });
        }
    }
}
