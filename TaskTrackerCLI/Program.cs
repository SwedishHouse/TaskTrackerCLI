using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
//using static System.Runtime.InteropServices.JavaScript.JSType;


namespace TaskTrackerCLI
{
    public class Task
    {
        public int Id { get; set; }

        private string desc;
        public string Descriptor
        { 
            get
            { 
                return desc;
            } 
            set {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("Invalid value!");
                }
                desc = value;
            } 
        }
        private enum TaskStatus
        {
            TODO,
            PROGRESS,
            DONE
        };

        private TaskStatus current_status = TaskStatus.TODO;

        private readonly Dictionary<TaskStatus, string> _statusTask = new Dictionary<TaskStatus, string>
        {
            { TaskStatus.TODO , "todo" },
            { TaskStatus.PROGRESS,"progress" },
            { TaskStatus.DONE, "done" }
        };
        public string StatusStr {
            get
            {
                return _statusTask[current_status];
            }
            set 
            {
                foreach(TaskStatus key in _statusTask.Keys)
                {
                    if (value == _statusTask[key])
                    {
                        _statusTask[key] = value;
                        break;
                    }
                }
            } 
        }

        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset UpdatedAt { get; set; }
        
        //
        public Task(int id, string desc, string status = "todo")
        {
            this.Id = id;
            this.Descriptor = desc;
            this.StatusStr = status;
            // Сохраняем время создания
            this.CreatedAt = this.UpdatedAt = DateTimeOffset.Now;
        }

        void Show()
        {
            Console.WriteLine(this.Id + " " + this.Descriptor + " " + this.StatusStr);
        }

    }

    public class Content
    {
        // Путь к файлу с задачами 
        public const string json_path = "tasks.json";
        // Хранение задач в оперативных данных
        public List<Task> Tasks { get; set; }
        // Список допустимых команд 
        public Dictionary<string, Action<string[]>> Commands { get; }

        

        public Content()
        {
            if (!File.Exists(json_path))
            {
                // Создадим пустой список для начального содержимого
                var initialData = new List<Task>();
                string jsonString = JsonSerializer.Serialize(initialData, new JsonSerializerOptions { WriteIndented = true });

                // Создаем файл, если он не создан
                File.WriteAllText(json_path, jsonString);
            }

            // Считываем весь текст из файла
            string jsonContent = File.ReadAllText(json_path);

            // Десериализуем данные в словарь
            Tasks = JsonSerializer.Deserialize<List<Task>>(jsonContent);

            // Создаем список методов для исполнения
            this.Commands = new Dictionary<string, Action<string[]>>
            {
                {"help",  this.DisplayHelp},    //Отображение справки
                {"add", this.Add }              // Добавление задачи
            };

        }// конец конструктора

        public void DisplayHelp(string[] args)
        {
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("  help  - Показать справку.");
            Console.WriteLine("  add   - Добавить элемент.");
            Console.WriteLine("  update {id:int} {task:string} - Обновить задачу.");
            Console.WriteLine("  delete {id:int} - Удалить задачу.");
            Console.WriteLine("  list  - Показать список.");
        }

        void Add(string[] args)
        {

            Task task = new Task(this.Tasks.Count + 1, args[0], "todo");
            try
            {
                this.Tasks.Add(task);
            }
            catch
            {
                Console.WriteLine("Some exception!");
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            // Форматируем шапку программы
            if (args.Length == 0)
            {
                Console.WriteLine("Пустой ввод!");
                return;
            }

            Content content = new();

            string command = args[0];

            // Извлекаем остальные аргументы
            string[] commandArgs = args.Skip(1).ToArray();

            if (content.Commands.ContainsKey(command))
            {
                // Вызываем метод, связанный с командой
                content.Commands[command](commandArgs);
            }
            else
            {
                Console.WriteLine($"Неизвестная команда: {command}");
                content.DisplayHelp(commandArgs);
            }
        }
    }
}

