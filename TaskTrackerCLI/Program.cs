using System;
using System.Text.Json;


namespace TaskTrackerCLI
{
    
    public class Task
    {
        private int id;
        public int Id 
        { 
            get => id;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("Value must be positive");
                this.id = value;
            } 
        }

        private string desc = "";

        public string Desc
        { 
            get => desc;
            set 
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("Invalid value!");
                }
                this.desc = value;
            } 
        }

        private readonly HashSet<string> _statusTasks = new HashSet<string>
        {
           "todo",
           "progress",
           "done" 
        };

        private string _status;
        public string StatusStr {
            get => _status;
            set 
            {
                if (!this._statusTasks.Contains(value))
                    throw new ArgumentException($"Values must be {this._statusTasks.ToString()}.\n\r But has {value}!!!");
                this._status = value;
            } 
        }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        //
        public Task(int id, string descriptor, string status="todo")
        {
            this.Id = id;
            this.Desc = descriptor;
            this.StatusStr = status;
            // Сохраняем время создания
            this.CreatedAt = this.UpdatedAt = DateTimeOffset.Now;
        }

        public override string ToString()
        {
            return $"{this.Id} {this.Desc} {this.StatusStr}";
        }

    }

    public class Content
    {
        // Путь к файлу с задачами 
        private readonly string json_path = "tasks.json";
        // Хранение задач в оперативных данных
        public List<Task> Tasks { get; set; }
        // Список допустимых команд 
        public Dictionary<string, Action<string[]>> Commands { get; }
        private readonly JsonSerializerOptions opt;

        public Content(string path)
        {
            json_path = path;
            this.opt = new JsonSerializerOptions { WriteIndented = true };

            if (!File.Exists(json_path))
            {
                // Создадим пустой список для начального содержимого
                var initialData = new List<Task>();
                string jsonString = JsonSerializer.Serialize(initialData, opt);

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
                {"add", this.Add },             // Добавление задачи
                {"delete", this.Delete },       // Удаление задачи
                {"list", this.List }            // Удаление задачи
            };
        }

        public Content() : this("tasks.json")
        {
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
            Task task = new(this.Tasks.Count + 1, args[0], "todo");
            try
            {
                this.Tasks.Add(task);
                string jsonString = JsonSerializer.Serialize<List<Task>>(this.Tasks, this.opt);

                File.WriteAllText(this.json_path, jsonString);
            }
            catch
            {
                Console.WriteLine("Some exception!");
            }
        }

        void Delete(string[] args)
        {

        }

        void List(string[] args)
        {
            if (args.Length == 0)
            {
                foreach (Task task in this.Tasks)
                {
                    Console.WriteLine(task.ToString());
                }
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

