using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace organizer_application.Models
{
   public class TaskModel : INotifyPropertyChanged
    {
        public int Id { get; set; } // Уникальный идентификатор задачи
        public string Title { get; set; } // Название задачи
        public string Description { get; set; } // Описание задачи
        public DateTime DueDate { get; set; } // Дата и время выполнения задачи
        public string Priority { get; set; } // Приоритет задачи (высокий, средний, низкий)
        public string Category { get; set; } // Категория задачи
        public bool IsCompleted { get; set; } // Для отметки выполнения
        public TimeSpan ReminderOffset { get; set; } = TimeSpan.FromMinutes(10);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _status;
        public string Status  // Новое свойство для статуса задачи
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public override string ToString()
        {
            // Для отображения в списке: название, дата и приоритет
            return $"{Title} - {DueDate:dd.MM.yyyy} - {Priority}";
        }
    }
}
