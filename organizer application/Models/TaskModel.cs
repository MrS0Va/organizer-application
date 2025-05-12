using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace organizer_application.Models
{
   public class TaskModel
    {
        public int Id { get; set; } // Уникальный идентификатор задачи
        public string Title { get; set; } // Название задачи
        public string Description { get; set; } // Описание задачи
        public DateTime DueDate { get; set; } // Дата и время выполнения задачи
        public string Priority { get; set; } // Приоритет задачи (высокий, средний, низкий)
        public string Category { get; set; } // Категория задачи
        public bool IsCompleted { get; set; } // Статус выполнения задачи (выполнена или нет)
        public override string ToString()
        {
            // Для отображения в списке: название, дата и приоритет
            return $"{Title} - {DueDate:dd.MM.yyyy} - {Priority}";
        }
    }
}
