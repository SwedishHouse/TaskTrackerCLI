using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace TaskTrackerCLI
{
    public class Task
    {
        public string Value { get; set; }
        public string Status { get; set; }

        public Task(string value = "", string status = "")
        {
            Value = value;
            Status = status;
        }
    }

    public class Content
    {
        // Путь к файлу с задачами 
        public const string json_path = "tasks.json";
        // Хранение задач в оперативных данных
        public Dictionary<int, Task> Tasks { get; set; }
        // Список допустимых команд 
        public Dictionary<string, Action<string[]>> Commands;

        public Content()
        {
            Tasks = new Dictionary<int, Task>();

            if (!File.Exists(json_path))
            {
                // Создадим пустой список для начального содержимого
                var initialData = new Dictionary<int, Task>();
                string jsonString = JsonSerializer.Serialize(initialData, new JsonSerializerOptions { WriteIndented = true });

                // Создаем файл, если он не создан
                File.WriteAllText(json_path, jsonString);
            }

            // Считываем весь текст из файла
            string jsonContent = File.ReadAllText(json_path);

            // Десериализуем данные в словарь
            Tasks = JsonSerializer.Deserialize<Dictionary<int, Task>>(jsonContent);

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

            Task task = new Task(args[0], "todo");

            try
            {
                this.Tasks.Add(this.Tasks.Count + 1, task);
            }
            catch {
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

